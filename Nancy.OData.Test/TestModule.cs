using System.Collections.Generic;

namespace Nancy.OData.Test
{
    public class TestModule : NancyModule
    {
        public TestModule()
            : base("/")
        {
            Get["/Test"] = x =>
            {
                return Response.AsOData(new List<Stuff> {
                new Stuff {             Name = "one" }, 
                     new Stuff { Name = "two" }, 
                        new Stuff { Name = "Three"}
                });
            };
        }
    }

    public class Stuff
    {
        public string Name { get; set; }
    }
}
