using HIMIS_API.Data;
using HIMIS_API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HIMIS_API.Services.MongoServices
{
    public class WorkPhysicalProgressService
    {
        private readonly IMongoCollection<Mongo_WorkPhysicalProgressModel> _collection;

        public WorkPhysicalProgressService(MongoDbContext dbContext)
        {
            _collection = dbContext.WorkPhysicalProgressFiles;
        }

        public async Task CreateAsync(Mongo_WorkPhysicalProgressModel document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task<Mongo_WorkPhysicalProgressModel> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(d => d.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<Mongo_WorkPhysicalProgressModel>> GetBySRAsync(string sr)
        {
            return await _collection.Find(d => d.SR == sr).ToListAsync();
        }

        //public async Task UpdateAsync(ObjectId id, Mongo_WorkPhysicalProgressModel updatedDocument)
        //{
        //    await _collection.ReplaceOneAsync(d => d.Id == id, updatedDocument);
        //}

        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(d => d.ID == id);
        }

        public async Task<List<Mongo_WorkPhysicalProgressModel>> GetAllAsync()
        {
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public  String GetByIDmTypeAsync(string sr,string imgName)
        {
            string strBytes = "";
            MongoDB_Class mongo = new MongoDB_Class();
            strBytes =  mongo.WorkPhysicalProgressFiles(sr, imgName);
            return strBytes;
            //return await _collection.Find(d => d.SR == sr).ToListAsync();
        }
    }
}
