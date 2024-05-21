// See https://aka.ms/new-console-template for more information
using ConsoleApp2;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

List<Car> myCars = new List<Car>(){
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };

var query1 = from car in myCars
             where car.Model == "A6"
             select new
             {
                 engineType = car.Engine.Model == "TDI" ? "diesel" : "petrol",
                 hppl = car.Engine.HorsePower / car.Engine.Displacement
             };

var query2 = from car in query1
             group car by car.engineType into engineGroup
             select new
             {
                 EngineType = engineGroup.Key,
                 AverageHppl = engineGroup.Average(car => car.hppl)
             };

foreach (var group in query1)
{
    Console.WriteLine($"{group.engineType}: {group.hppl}");
}


foreach (var group in query2)
{
    Console.WriteLine($"{group.EngineType}: {group.AverageHppl}");
}


SerializeCars(myCars);



List<Car> deserializedCars = DeserializeCars();



XElement rootNode = XElement.Load("CarsCollection.xml");

double avgHP = rootNode.XPathSelectElements("//Car[Engine/Model != 'TDI']/Engine/HorsePower")
    .Select(x => (double)x).Average();


Console.Out.WriteLine("Średnia policzona z XPathSelectElements: " + avgHP.ToString());

IEnumerable<string> models = rootNode.XPathSelectElements("//Car/Model").Select(x => x.Value).Distinct();

foreach (string model in models)
{
    Console.WriteLine(model);
}


createXmlFromLinq(myCars);

GenerateXHTMLTable(myCars);


ModifyXmlDocument();


static void SerializeCars(List<Car> cars)
{
    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
    using (TextWriter writer = new StreamWriter("CarsCollection.xml"))
    {
        serializer.Serialize(writer, cars);
    }
}


static List<Car> DeserializeCars()
{
    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
    using (FileStream fileStream = new FileStream("CarsCollection.xml", FileMode.Open))
    {
        return (List<Car>)serializer.Deserialize(fileStream);
    }
}


static void createXmlFromLinq(List<Car> myCars)
{
    IEnumerable<XElement> nodes = from car in myCars
                select new XElement("car",
                            new XElement("model", car.Model),
                            new XElement("engine",
                                new XAttribute("model", car.Engine.Model),
                                new XElement("displacement", car.Engine.Displacement),
                                new XElement("horsePower", car.Engine.HorsePower)
                                ),
                            new XAttribute("year", car.Year)
                );

    XElement rootNode = new XElement("cars", nodes);
    rootNode.Save("CarsFromLinq.xml");
}

static void GenerateXHTMLTable(List<Car> myCars)
{
    XElement table = new XElement("table",
                        new XElement("tr",
                            new XElement("th", "Model"),
                            new XElement("th", "Engine Model"),
                            new XElement("th", "Displacement"),
                            new XElement("th", "Horse Power"),
                            new XElement("th", "Year")
                        ),
                        from car in myCars
                        select new XElement("tr",
                                    new XElement("td", car.Model),
                                    new XElement("td", car.Engine.Model),
                                    new XElement("td", car.Engine.Displacement),
                                    new XElement("td", car.Engine.HorsePower),
                                    new XElement("td", car.Year)
                                )
                    );

    XDocument xhtmlDoc = new XDocument(
        new XElement("html",
            new XElement("head"),
            new XElement("body", table)
        )
    );

    xhtmlDoc.Save("CarsTable.xhtml");
}


static void ModifyXmlDocument()
{
    XElement root = XElement.Load("CarsCollection.xml");

    foreach (var carElement in root.Elements("Car"))
    {
        XElement horsePowerElement = carElement.Element("Engine").Element("HorsePower");
        if (horsePowerElement != null)
        {
            horsePowerElement.Name = "hp";
        }

        XElement yearElement = carElement.Element("Year");
        if (yearElement != null)
        {
            string yearValue = yearElement.Value;
            carElement.Element("Model").SetAttributeValue("Year", yearValue);
            yearElement.Remove();
        }
    }

    root.Save("ModifiedCarsCollection.xml");
}