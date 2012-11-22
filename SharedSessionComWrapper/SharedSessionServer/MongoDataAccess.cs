using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOL.SharedSessionServer
{
    public class MongoDataAccess
    {
        string _connectionString = "";
        string nameCollection = "Sessions";
        public MongoDataAccess(string cnnstring)
        {
            _connectionString = cnnstring;
           
        }

        private MongoServer GetConnection()
        {
            return MongoServer.Create(_connectionString);
        }

        private MongoCollection<BsonDocument> GetSessionCollection(MongoServer conn)
        {
            return conn.GetDatabase("SessionState").GetCollection(nameCollection);
        }

        public void SetData(string IdSession, string data, DateTime lastAccess)
        {
            MongoServer conn = GetConnection();
            MongoCollection sessionCollection = GetSessionCollection(conn);
            BsonDocument insertDoc = null;
            SafeMode _safeMode = new SafeMode(false);

            string sessItems = data;
            var ApplicationName = "IOL";
            Double timeout = this.GetTimeoutSet();
            object lockId = new object();

            
            try
            {
                if (!this.Exist(conn, IdSession, sessionCollection))
                {
                    insertDoc = new BsonDocument();
                    insertDoc.Add("_id", IdSession);
                    insertDoc.Add("ApplicationName", ApplicationName);
                    insertDoc.Add("Created", DateTime.Now.ToUniversalTime());
                    insertDoc.Add("Expires", DateTime.Now.AddMinutes((Double)timeout).ToUniversalTime());
                    insertDoc.Add("SessionItems", sessItems);

                    var query = Query.And(Query.EQ("_id", IdSession), Query.EQ("ApplicationName", ApplicationName), Query.LT("Expires", DateTime.Now.ToUniversalTime()));
                    sessionCollection.Remove(query, _safeMode);
                    sessionCollection.Insert(insertDoc, _safeMode);
                }
                else
                {
                    var query = Query.And(Query.EQ("_id", IdSession), Query.EQ("ApplicationName", ApplicationName));
                    var update = Update.Set("Expires", DateTime.Now.AddMinutes((Double)timeout).ToUniversalTime());
                    update.Set("SessionItems", sessItems);
                    sessionCollection.Update(query, update, _safeMode);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                conn.Disconnect();
            }
        }

        private bool Exist(MongoServer conn, string IdSession, MongoCollection sessionCollection)
        {
            var result = GetByIdSession(IdSession, sessionCollection);


            if (result == null) return false;
            return result.Count() > 0;
        }

        private BsonDocument GetByIdSession(string IdSession, MongoCollection sessionCollection)
        {
            var query = Query.And(Query.EQ("_id", IdSession));

            var result = sessionCollection.FindOneAs<BsonDocument>(query);

            return result;
        }

        public void UpdateLastAccess(string IdSession, DateTime lastAccess)
        {


            MongoServer conn = GetConnection();
            MongoCollection sessionCollection = GetSessionCollection(conn);

            var ApplicationName = "IOL";
            Double timeout = this.GetTimeoutSet();
            object lockId = new object();

            SafeMode _safeMode = new SafeMode(false);

            try
            {

                var query = Query.And(Query.EQ("_id", IdSession), Query.EQ("ApplicationName", ApplicationName));
                var update = Update.Set("Expires", DateTime.Now.AddMinutes((Double)timeout).ToUniversalTime());

                update.Set("Locked", false);
                sessionCollection.Update(query, update, _safeMode);

            }
            catch (Exception e)
            {

            }
            finally
            {
                conn.Disconnect();
            }
        }

        private double GetTimeoutSet()
        {
            return 480;
        }

        public string GetSessionData(string sessionId)
        {
            MongoServer conn = GetConnection();
            MongoCollection sessionCollection = GetSessionCollection(conn);


            string resultSessionItems = null;

            try
            {

                var result = GetByIdSession(sessionId, sessionCollection);

                if (result != null)
                {
                    var ret = result["SessionItems"];
                    resultSessionItems = ret.AsString; ;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                conn.Disconnect();
            }

            return resultSessionItems;
        }

        public void DeleteSessionData(string sessionId)
        {

        }

    }
}
