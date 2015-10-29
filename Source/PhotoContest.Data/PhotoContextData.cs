namespace PhotoContest.Data
{
    using System;
    using System.Collections.Generic;
    
    using Contracts;
    using Models;

    public class PhotoContextData : IPhotoContestData
    {
        private IPhotoContestContext context;
        private IDictionary<Type, object> repositories;

        public PhotoContextData(IPhotoContestContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<User> Users
        {
            get { return this.GetRepository<User>(); }
        }

        public IRepository<Contest> Contests
        {
            get { return this.GetRepository<Contest>(); }
        }

        public IRepository<Picture> Pictures
        {
            get { return this.GetRepository<Picture>(); }
        }

        public IRepository<Prize> Prizes
        {
            get { return this.GetRepository<Prize>(); }
        }

        public IRepository<Vote> Votes
        {
            get { return this.GetRepository<Vote>(); }
        }

        public IRepository<VotingCommittee> Committees
        {
            get { return this.GetRepository<VotingCommittee>(); }
        }

        public IRepository<Notification> Notifications
        {
            get { return this.GetRepository<Notification>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }
    }
}
