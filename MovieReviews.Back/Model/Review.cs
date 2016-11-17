using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;

namespace MovieReviews.Back.Model
{
    public class Review
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual string Text { get; set; }
    }

    public class ReviewMap : ClassMapping<Review>
    {
        public ReviewMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.GuidComb));
            Property(x => x.Title);
            Property(x => x.Text);
            ManyToOne(x => x.Movie, x =>
            {
                x.PropertyRef("Id");
                x.Column("Movie");
            });
        }
    }
}
