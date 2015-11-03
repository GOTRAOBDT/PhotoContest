namespace PhotoContest.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PhotoContest.Data.Contracts;

    public class GenericRepositoryMock<T> : IRepository<T> 
        where T : class
    {
        private List<T> entities = new List<T>();

        private Func<T, object> keySelector; 

        public bool ChangesSaved { get; set; }

        public GenericRepositoryMock(Func<T, object> keySelector = null)
        {
            if (keySelector != null)
            {
                this.keySelector = keySelector;
            }
            else
            {
                this.keySelector = (u => ((dynamic)u).Id);
            }
        }

        public IQueryable<T> All()
        {
            return this.entities.AsQueryable();
        }

        public T Find(object id)
        {
            return this.entities.FirstOrDefault(entity => id.Equals(this.keySelector(entity)));
        }

        public void Add(T entity)
        {
            this.entities.Add(entity);
        }

        public void Update(T entity)
        {
            var existingEntity = this.Find(this.keySelector(entity));
            var existingEntityIndex = this.entities.IndexOf(existingEntity);
            this.entities[existingEntityIndex] = entity;
        }

        void IRepository<T>.Delete(T entity)
        {
            this.entities.Remove(entity);
        }

        public int SaveChanges()
        {
            this.ChangesSaved = true;
            return 1;
        }


        public T Delete(T entity)
        {
            var existingEntity = this.Find(this.keySelector(entity));
            var existingEntityIndex = this.entities.IndexOf(existingEntity);
            this.entities.RemoveAt(existingEntityIndex);
            return existingEntity;
        }


        public T Delete(object Id)
        {
            var existingEntity = this.Find(Id);
            var existingEntityIndex = this.entities.IndexOf(existingEntity);
            this.entities.RemoveAt(existingEntityIndex);
            return existingEntity;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
