using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DiscordClients.Core.SQL.Tables;

using LiteDB;

namespace DiscordClients.Core.SQL
{
    public class SQLite
    {
        public LiteDatabase db;
        public SQLite(string path)
        {
            if (db == null)
                db = new LiteDatabase($"Filename={path};connection=shared");
            else
                throw new Exception("Connection already exists");
        }

        /// <summary>
        /// Updates one
        /// </summary>
        /// <typeparam name="T">Type should be a SqlDocument</typeparam>
        /// <param name="entity">Document</param>
        /// <returns>Returns bool value true and false if document wasn't fount</returns>
        public bool Update<T>(T entity) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Update(entity);
        }

        /// <summary>
        /// Updates an list of documents
        /// </summary>
        /// <typeparam name="T">Type should be a SqlDocument</typeparam>
        /// <param name="entityList">Document list </param>
        /// <returns>Returns the count of updates documents</returns>
        public int Update<T>(IEnumerable<T> entityList) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Update(entityList);
        }

        /// <summary>
        /// Updates an list of documents
        /// </summary>
        /// <typeparam name="T">Type should be a SqlDocument</typeparam>
        /// <param name="entityList">Document list </param>
        /// <returns>Returns the count of updates documents</returns>
        public int UpdateMany<T>(BsonExpression transform, BsonExpression predicate) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.UpdateMany(transform, predicate);
        }

        public ObjectId Insert<T>(T entity) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            ObjectId objectId = col.Insert(entity);
            return objectId;
        }

        public int Insert<T>(IEnumerable<T> entityList) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Insert(entityList);
        }

        public int UpsertList<T>(IEnumerable<T> entityList) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Upsert(entityList);
        }

        public bool Upsert<T>(T entity) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Upsert(entity);
        }

        public bool Delete<T>(ObjectId id) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Delete(id);
        }

        public int DeleteAll<T>() where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.DeleteAll();
        }

        public int DeleteMany<T>(Expression<Func<T, bool>> predicate) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.DeleteMany(predicate);
        }

        public IEnumerable<T> FindAll<T>() where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.FindAll();
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.Find(predicate);
        }

        public T FindOne<T>(Expression<Func<T, bool>> predicate) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.FindOne(predicate);
        }

        public T FindById<T>(BsonValue val) where T : SqlDocument
        {
            var col = db.GetCollection<T>();
            return col.FindById(val);
        }
    }
}
