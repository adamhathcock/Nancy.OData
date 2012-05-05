using System.Collections.Generic;

namespace Nancy.OData.Test
{
    public class TestModule : NancyModule
    {
        public TestModule()
            : base("/")
        {
            Get["/Test/{stuff}/"] = x =>
            {
                return Response.AsOData(new List<Stuff> {
                new Stuff {Id = 1, Name = "one" }, 
                     new Stuff {Id = 2, Name = "two" }, 
                        new Stuff {Id = 3, Name = "Three"}
                });
            };
        }
    }

    public class Stuff
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
