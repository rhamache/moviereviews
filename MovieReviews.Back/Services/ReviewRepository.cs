using System;
using MovieReviews.Back.Model;
using NHibernate;

namespace MovieReviews.Back.Services
{
    public class ReviewRepository : IReviewRepository
    {
        public void Delete(Review review)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete(review);
                transaction.Commit();
            }
        }

        public Review Get(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.Get<Review>(id);
        }

        public long RowCount()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.QueryOver<Review>().RowCountInt64();
            }
        }

        public void Save(Review review)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(review);
                transaction.Commit();
            }
        }

        public void Update(Review review)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(review);
                transaction.Commit();
            }
        }
    }
}
