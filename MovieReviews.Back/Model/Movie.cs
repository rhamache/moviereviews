using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Model
{
    public class Movie
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string ImdbKey { get; set; }
        public virtual TimeSpan RunningTime { get; set; }
        public virtual DateTime ReleaseDate { get; set; }
    }

    public class MovieMap : ClassMapping<Movie>
    {
        public MovieMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.GuidComb));
            Property(x => x.Title);
            Property(x => x.ImdbKey);
            Property(x => x.RunningTime);
            Property(x => x.ReleaseDate);
        }
    }
}
