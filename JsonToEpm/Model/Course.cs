using System;

namespace PsvJsonToPlex.Model
{
    public class Course
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
    }
}