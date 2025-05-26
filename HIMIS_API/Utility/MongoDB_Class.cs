using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;
using MongoDB.Shared;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using System.IO;
/// <summary>
/// Summary description for MongoDB_Class
/// </summary>
public class Student_Info
{
    public ObjectId _id { get; set; }

    public Int32 FileID { get; set; }

    public Int32 Student_ID { get; set; }

    public Int32 SR { get; set; }
    public Int32 EXTID { get; set; }

    public Int32 ESTAPRID { get; set; }

    public Int32 SampleID { get; set; }

    public Int32 DHSAIID { get; set; }
    public Int32 ReceiptID { get; set; }


    public String Name { get; set; }
    public String FileName { get; set; }

    public String ImageName { get; set; }
    public String ImageName2 { get; set; }
    public String ImageName3 { get; set; }
    public String ImageName4 { get; set; }
    public String ImageName5 { get; set; }

    public String ext { get; set; }

    public String Work_id { get; set; }





    public int Class { get; set; }
    public String Subject { get; set; }

    public BsonBinaryData Bty { get; set; }
    public BsonBinaryData File { get; set; }

    public BsonBinaryData ImageData { get; set; }
    public BsonBinaryData ImageDatalvl2 { get; set; }
    public BsonBinaryData ImageDatalvl3 { get; set; }
    public BsonBinaryData ImageDatalvl4 { get; set; }
    public BsonBinaryData ImageDatalvl5 { get; set; }
    public BsonBinaryData FileWLabel { get; set; }

}
public class MongoDB_Class
{
    MongoServerSettings Settings_;
    MongoServer server;
    MongoDatabase Database_;
    public static MongoDB_Class _Obj;
    public static MongoDB_Class GetObject()
    {
        if (_Obj == null)
        {
            _Obj = new MongoDB_Class();
        }

        return _Obj;
    }

    public MongoDB_Class()
    {
        Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        // Database_ = server.GetDatabase("Temp");

        Database_ = server.GetDatabase("HIMIS");


    }


    public void Insert_ProgressImage(Student_Info _Obj)
    {

        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("WorkPhysicalProgressFiles");
            BsonDocument Stu_Doc = new BsonDocument { };


            Stu_Doc.Add("ext", _Obj.ext);
            Stu_Doc.Add("Work_id", _Obj.Work_id);
            Stu_Doc.Add("SR", _Obj.SR);




            if (_Obj.ImageData != null)
            {
                Stu_Doc.Add("ImageName", _Obj.ImageName);
                Stu_Doc.Add("ImageData", _Obj.ImageData);
            }
            if (_Obj.ImageDatalvl2 != null)
            {
                Stu_Doc.Add("ImageName2", _Obj.ImageName2);
                Stu_Doc.Add("ImageDatalvl2", _Obj.ImageDatalvl2);
            }

            if (_Obj.ImageDatalvl3 != null)
            {
                Stu_Doc.Add("ImageName3", _Obj.ImageName3);
                Stu_Doc.Add("ImageDatalvl3", _Obj.ImageDatalvl3);
            }

            if (_Obj.ImageDatalvl4 != null)
            {
                Stu_Doc.Add("ImageName4", _Obj.ImageName4);
                Stu_Doc.Add("ImageDatalvl4", _Obj.ImageDatalvl4);
            }

            if (_Obj.ImageDatalvl5 != null)
            {
                Stu_Doc.Add("ImageName5", _Obj.ImageName5);
                Stu_Doc.Add("ImageDatalvl5", _Obj.ImageDatalvl5);
            }
            // BsonDocument Stu_Doc = new BsonDocument{
            //                       {"ABGID",_Obj.ReceiptID} ,
            //                       {"FileName",_Obj.FileName},
            //                       {"File",_Obj.File},
            //                       { "ext",_Obj.ext}


            //};


            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }

    public List<Student_Info> Retrieve_Installation_File(Student_Info _Obj)
    {
        try
        {



            List<Student_Info> Student_List = new List<Student_Info>();
            var StuInfo = Database_.GetCollection<Student_Info>("SupQCinvoicefiles");
            int idd = _Obj.Student_ID;
            var ids = new int[] { idd };
            var query = Query.In("deliveryplanid", BsonArray.Create(ids));
            var items = StuInfo.Find(query);



            foreach (Student_Info Stu in items)
            {
                Student_List.Add(Stu);
            }

            return Student_List;
        }
        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }


    public void Insert_Ext_File(Student_Info _Obj)
    {

        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("WorkExtensionFile");
            BsonDocument Stu_Doc = new BsonDocument { };



            Stu_Doc.Add("Work_id", _Obj.Work_id);
            Stu_Doc.Add("EXTID", _Obj.EXTID);
            Stu_Doc.Add("FileName", _Obj.FileName);
            Stu_Doc.Add("File", _Obj.File);
            Stu_Doc.Add("ext", _Obj.ext);

            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }


    public string Work_Ext_Files(string theID, string fname)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("HIMIS");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("WorkExtensionFile");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("EXTID", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {

                Dat = Convert.ToBase64String((byte[])Stu.File);





                //   img = new MemoryStream((byte[])Stu.File);

            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }


    public List<Student_Info> Retrieve_Student_File()
    {
        try
        {
            server.Connect();
            List<Student_Info> Student_List = new List<Student_Info>();
            var StuInfo = Database_.GetCollection<Student_Info>("StudentFile");

            var ids = new int[] { 570 };
            var query = Query.In("ReceiptID", BsonArray.Create(ids));
            var items = StuInfo.Find(query);



            foreach (Student_Info Stu in items)
            {
                Student_List.Add(Stu);
            }

            return Student_List;
        }
        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }

    public List<Student_Info> Retrieve_Student_Information()
    {
        try
        {
            server.Connect();
            List<Student_Info> Student_List = new List<Student_Info>();
            var StuInfo = Database_.GetCollection<Student_Info>("Student_Information");

            foreach (Student_Info Stu in StuInfo.FindAll())
            {
                Student_List.Add(Stu);
            }

            return Student_List;
        }
        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }

    public void Insert_Student_Information(Student_Info _Obj)
    {
        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("Student_Information");
            BsonDocument Stu_Doc = new BsonDocument{
                                   {"Student_ID",_Obj.Student_ID} ,
                                   {"Name",_Obj.Name},
                                   {"Class",_Obj.Class},
                                   {"Subject",_Obj.Subject}

            };
            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }


    public void Insert_QCNSQ_File(Student_Info _Obj)
    {

        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("QCNSQFile");
            BsonDocument Stu_Doc = new BsonDocument{
                                   {"SampleID",_Obj.Student_ID} ,

                                   {"FileName",_Obj.FileName},
                                     {"File",_Obj.File},
                                   { "ext",_Obj.Subject}



            };
            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }

    public void Insert_DHSIndent_File(Student_Info _Obj)
    {

        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("DHSAnnualindentFile");
            BsonDocument Stu_Doc = new BsonDocument{
                                   {"DHSAIID",_Obj.Student_ID} ,

                                   {"FileName",_Obj.FileName},
                                     {"File",_Obj.File},
                                   { "ext",_Obj.Subject}



            };
            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }

    public void Delete_Student_Infromation(Student_Info _Obj)
    {
        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("Student_Information");
            IMongoQuery Marker = Query.EQ("Student_ID", _Obj.Student_ID);
            Collection_.Remove(Marker);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }















    public string DisplayImage(string theID, string fname, string Ftype)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("CGMSCL");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("masPharmacopeiaFiles");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("pharmaid", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {
                if (Ftype == "FLabel")
                {

                    Dat = Convert.ToBase64String((byte[])Stu.File);
                    //   img = new MemoryStream((byte[])Stu.File);
                }
                else
                {
                    Dat = Convert.ToBase64String((byte[])Stu.FileWLabel);

                }
            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }






    public string DisplayImagePreBID(string theID, string fname, string Ftype)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("CGMSCL");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("masPrebid");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("pharmaid", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {
                if (Ftype == "FLabel")
                {

                    Dat = Convert.ToBase64String((byte[])Stu.File);
                    //   img = new MemoryStream((byte[])Stu.File);
                }
                else
                {
                    Dat = Convert.ToBase64String((byte[])Stu.FileWLabel);

                }
            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }


    public string DisplayImageNIB(string theID, string fname)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("CGMSCL");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("MASSONIBREPORTFiles");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("NIBID", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {

                Dat = Convert.ToBase64String((byte[])Stu.File);
                //   img = new MemoryStream((byte[])Stu.File);

            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }

    //public void Insert_NIBReport_File(Student_Info _Obj)
    //{

    //    try
    //    {
    //        server.Connect();
    //        MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("MASSONIBREPORTFiles");
    //        BsonDocument Stu_Doc = new BsonDocument{
    //                               {"NIBID",_Obj.Student_ID} ,
    //                               {"ponoid",_Obj.ReceiptID} ,
    //                               {"FileName",_Obj.FilenameInvoice},
    //                                 {"File",_Obj.FileInvoice},


    //                               { "ext",_Obj.Subject}



    //        };
    //        Collection_.Insert(Stu_Doc);
    //    }

    //    catch
    //    {
    //        throw;
    //    }
    //    finally
    //    {
    //        server.Disconnect();
    //    }
    //}

    //public void Update_Pharmacopiea_File(Student_Info _Obj)
    //   {
    //       try
    //       {
    //           server.Connect();
    //           MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("masPharmacopeiaFiles");
    //           IMongoQuery Marker = Query.EQ("pharmaid", _Obj.Student_ID);


    //           IMongoUpdate Update_ = MongoDB.Driver.Builders.Update.Set("FileName", _Obj.FilenameInvoice)
    //                                                               .Set("File", _Obj.FileInvoice)
    //                                                               .Set("FileNameWLabel", _Obj.FileNameWLabel)
    //                                                               .Set("FileWLabel",_Obj.FileWLabel)
    //                                                               .Set("ext", _Obj.Subject);

    //           Collection_.Update(Marker, Update_);
    //       }

    //       catch
    //       {
    //           throw;
    //       }
    //       finally
    //       {
    //           server.Disconnect();
    //       }

    //   }

    public string WorkPhysicalProgressFiles(string theID, string mType)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("HIMIS");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("WorkPhysicalProgressFiles");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("SR", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {
                //if (mType == "Image1")
                if ( !string.IsNullOrEmpty(mType) && vv == 0)
                {
                    Dat = Convert.ToBase64String((byte[])Stu.ImageData);
                }

                if (mType == "Image2")
                {
                    Dat = Convert.ToBase64String((byte[])Stu.ImageDatalvl2);
                }
                if (mType == "Image3")
                {
                    Dat = Convert.ToBase64String((byte[])Stu.ImageDatalvl3);
                }
                if (mType == "Image4")
                {
                    Dat = Convert.ToBase64String((byte[])Stu.ImageDatalvl4);
                }
                if (mType == "Image5")
                {
                    Dat = Convert.ToBase64String((byte[])Stu.ImageDatalvl5);
                }


                //   img = new MemoryStream((byte[])Stu.File);

            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }


    public string Estimate_BOQ_Files(string theID, string fname)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("HIMIS");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("tblAprEstimateMasterFile");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("ESTAPRID", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {

                Dat = Convert.ToBase64String((byte[])Stu.File);





                //   img = new MemoryStream((byte[])Stu.File);

            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }



    public void Insert_BOQ_File(Student_Info _Obj)
    {

        try
        {
            server.Connect();
            MongoCollection<Student_Info> Collection_ = Database_.GetCollection<Student_Info>("tblAprEstimateMasterFile");
            BsonDocument Stu_Doc = new BsonDocument { };
            Stu_Doc.Add("ESTAPRID", _Obj.EXTID);
            Stu_Doc.Add("FileName", _Obj.FileName);
            Stu_Doc.Add("File", _Obj.File);
            Stu_Doc.Add("ext", _Obj.ext);
            Collection_.Insert(Stu_Doc);
        }

        catch
        {
            throw;
        }
        finally
        {
            server.Disconnect();
        }
    }



    public string SelfLifeImage(string theID)
    {

        MemoryStream img = null;
        MongoServer server;


        string Dat = "";
        byte[] imgw = null;


        MongoServerSettings Settings_ = new MongoServerSettings();
        Settings_.Server = new MongoServerAddress("localhost", 27017);
        server = new MongoServer(Settings_);
        MongoDatabase myDB = server.GetDatabase("CGMSCL");
        List<Student_Info> Student_List = new List<Student_Info>();

        var StuInfo = myDB.GetCollection<Student_Info>("tblselflifeextapplysupFile");

        var ids = new int[] { Convert.ToInt32(theID) };
        var query = MongoDB.Driver.Builders.Query.In("SLETID", BsonArray.Create(ids));
        var items = StuInfo.Find(query);



        foreach (Student_Info Stu in items)
        {
            for (int vv = 0; vv <= StuInfo.Count(); vv++)
            {


                Dat = Convert.ToBase64String((byte[])Stu.File);
                //   img = new MemoryStream((byte[])Stu.File);

            }

        }




        try
        {
            return Dat;
        }
        catch
        {
            return null;
        }
        finally
        {

        }
    }



}