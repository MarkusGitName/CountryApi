# CountryApi

Build .net 6 <br>
First time building in net 6 so the project took a bit longer than usually. I did however take advantage of the opertunity to explore .net 6 and whats new in the framework. <br>

[Demo](https://github.com/MarkusGitName/CountriesDemo)

# Testing

## Postman

I could not get the Github Action that runs the postman collection on any merge or push to master branch to run succesfully. Need a bit more time to automate the integration tests but the postman collection can be found in Postman folder and can be imported and run manually from Postman.

## Unit Testing

Unfortunatly because of time, I did not get to writing any Unit Tests. However, because of the scope of the project and the fact that there is not really complex computing happening and more focussed on Integration, I thought its better to focus on integration testing. 

# DataBase

Entity framework is used for DataBase interactions.
My Experience with Entity Framwork almost always pressents the same problem of self reference loops, and even though I have a couple sollutions I thought this to be a good opportunity to re-think my usual approuch.

This commands generate the scripts to create or the database
> add-migrataion migrationName

This command runs the scripts generated in the previous  command
> update-database

These scripts can be found in Migration folder in the project

## Models

Three distinct type of model klasses exist. Table, Database, and Transfer models. Table models directly represents the fields of each table in the database and contains no relationship information. The database models contains all the relationship within the database and inherints the field properties from table models. Confusingly the database models is named with 'Table' appended (ModelTable) because these models is used by entity framework and the database context to generate the actual sql tables.<br>
Lastly we have the transfer models witch is used when responding to requests. These models also inherits the field properties from the table models and contains the necesarry related objects, but the related objects only have the field properties present and no additional child properties. Thus no self referencing loops is possible.<br>

### Table Models

<code>

    public class Country
    {
        public Country(Guid id,string name,string alpha2,string alpha3,string numeric,bool active )
        {
            Id = id;
            Name = name;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            Numeric = numeric;
            Active = active;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Display(Name = "Alpha2")]
        public string? Alpha2 { get; set; }
        [Display(Name = "Alpha3")]
        public string? Alpha3 { get; set; }
        [Display(Name = "Numeric")]
        public string? Numeric { get; set; }
        [Display(Name = "Active")]
        public bool Active{ get; set; }
    }
    
    public class Currency
    {
        public Currency(Guid id, string name, bool active)
        {
            Id = id;
            Name = name;
            Active = active;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }
    }
    
</code>

### Database Models

<code>

    [Table("Country")]
    public class CountryTable : Country
    {
        public CountryTable(Guid id,string name, string alpha2, string alpha3, string numeric,bool active) : base(id,name, alpha2, alpha3, numeric,active)
        {
            Id = id;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            Numeric = numeric;
            Active = active;
            Currencies = new HashSet<CurrencyTable>();
        }

        public ICollection<CurrencyTable>? Currencies { get; set; }
    }
    
    [Table("Currency")]
    public class CurrencyTable : Currency
    {
        public CurrencyTable(Guid id,string name, bool active) : base(id,name,active)
        {
            Id = id;
            Name = name;
            Active = active;    
            Countries = new HashSet<CountryTable>();
        }
        public ICollection<CountryTable>? Countries { get; set; }
    }
</code>

### Transfer Models

<code>

    public class CountryTransfer : Country
    {
        public CountryTransfer(CountryTable country) : base(country.Id,country.Name, country.Alpha2, country.Alpha3, country.Numeric,country.Active)
        {
            Id= country.Id;
            Name = country.Name;
            Alpha2 = country.Alpha2;
            Alpha3 = country.Alpha3;
            Numeric = country.Numeric;
            Active = country.Active;
            foreach(CurrencyTable currency in country.Currencies)
            {
                Currencies.Add(new Currency(currency.Id,currency.Name, currency.Active));
            }
        }
        public List<Currency>? Currencies { get; set; } = new List<Currency>();
    }
    
    public class CurrencyTransfer: Currency
    {

        public CurrencyTransfer(CurrencyTable currency) : base(currency.Id,currency.Name, currency.Active)
        {
            Name = currency.Name;
            Id = currency.Id;

            foreach (CountryTable country in currency.Countries)
            {
                Countries.Add(new Country(country.Id,country.Name,country.Alpha2,country.Alpha3,country.Numeric,country.Active));
            }

        }
        public List<Country>? Countries { get; set; }= new List<Country>(); 
    }
</code>


# Endpoints

Full CRUD opperation was generated via Entity Framework. I tok the opertunity to see whats new in .net 6 and the code scaffolding template. With some minor adjustments and the model classes structure, I managed to include the first layer of child objects in requests or responses while avoiding self referenses.<br>

 ## api/Countries
 
 1. get // All
 2. get(guid Id) // ById
 3. post(Country object) // Create
 4. put // Edit
 5. get(string predicate)// URL-api/Countries/ByPredicate/{predicate} byPredicate: returns country whose Name, Alpha2, Alpha3 or Numeric fieds match the predicate.


## api/Currencies
 1. get // All
 2. get(guid Id) // ById
 3. post(Country object) // Create
 4. put // Edit
 5. get(string predicate) // CountriesByCurrency: returns all countries that make use of the currency specified with the predicate

## Swagger/openApi

The API description json document gets generated from the code allong with a gui where the endpoints can be explored and even tested. The gui and json can be found at https://localhost:7164/swagger/index.html.<br>
I make use of the generated json to build the postman collection by importing it into Postman, shuffeling the requests and adding Tests.


# Docker

Docker build command creates the docker image whitch is later used in docker-compose file.<br>
The -t tag is used to name the repo and image name for reference in the docker-compose file

> docker build -t reponame/reponame/imagename pathToDockefile
> docker build -t reponame/containername .

<code>

    FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
    WORKDIR /app
    EXPOSE 80
    EXPOSE 443

    FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
    WORKDIR /src
    COPY ["CountryApi/CountryApi.csproj", "CountryApi/"]
    RUN dotnet restore "CountryApi/CountryApi.csproj"
    COPY . .
    WORKDIR "/src/CountryApi"
    RUN dotnet build "CountryApi.csproj" -c Release -o /app/build

    FROM build AS publish
    RUN dotnet publish "CountryApi.csproj" -c Release -o /app/publish

    FROM base AS final
    WORKDIR /app
    COPY --from=publish /app/publish .
    ENTRYPOINT ["dotnet", "CountryApi.dll"]

</code>

## docker-compose

Make sure to cd to the directory that contains the compose file if the location is not specified in the command.

> docker-compose up

<code>

	version: '3.2'

	services:
	 
	  proxy:
		container_name: proxy
		image: nginx:latest
		networks:
		- countrynetwork
		ports:
		- "80:80"
		- "443:443"
		depends_on:
		- countryapi
		volumes:
		- ./Resources/nginx.conf:/etc/nginx/nginx.conf
		- /etc/letsencrypt/:/etc/letsencrypt/
		

	  countryapi:
		container_name: countryapi
		image: reponame/countryapi:latest
		environment:
		- ASPNETCORE_ENVIRONMENT=Development
		expose:
		- 80
		ports:
		- "81:80"
		networks:
		- countrynetwork
		depends_on:
		- potfoliodb
		volumes:
		- type: bind
		  source: ./uploads
		  target: /app/uploads
		  

	  countrydb:
		image: mcr.microsoft.com/mssql/server:2017-latest-ubuntu
		expose:
		- 1433
		ports:
		- "1400:1433"
		environment:
		- ACCEPT_EULA=Y
		- SA_PASSWORD=Markus@2
		- MSSQL_PID=Developer
		networks:
		- countrynetwork
		volumes:
		- type: bind
		  source: ./data
		  target: /var/opt/mssql/data
		- type: bind
		  source: ./log
		  target: /var/opt/mssql/log
		- type: bind
		  source: ./secrets
		  target: /var/opt/mssql/secrets

		

	networks:
	  countrynetwork:

</code>

## Nginx Config

The following command is run on the host to generates SSl certificates. Volume mapping is used to map the certificates from the host to the container. <br>
The below code snipped is a file named nginx.conf. Nginx makes use of it to pass requests on to the correct container. te conf file is also used to redirect requests from http to https ports.

> sudo -H ./letsencrypt-auto certonly --standalone -d <domain>.com -d www.<domain>

<code>

    events {
        worker_connections 1024;
    }

    http {
    server {
        server_name <serverUrl>;

        location / {
        return 301 https://$host$request_uri;
        }

    }
    server {
        server_name <serverUrl>;
        location / {
        proxy_pass http://countryapi:80/;           // container name in compose file and the exposed port
        }
        listen 80;
        listen 443 ssl;
        ssl_certificate /etc/letsencrypt/live/<domain>/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/<domain>/privkey.pem;
        include /etc/letsencrypt/options-ssl-nginx.conf;
    }
}

</code>

## docker Volumes

Die volume mapping word gebruik om data te persist as die container ge-restart moet word of net op failure. Databsis data word na die host to gemap in die countryapi container en dan word die ssl en nginx config files van die host af na die proxy container toe gemap.<br> 
Volume Mapping is used to persist data on the host machine incase of container restart or failure. The database files gets mapped from container to host to persist the database data and then nginx config file aswell as ssl certificates gets mapped from host to container as resurces.<br>

## Docker networking

 Looking at the comose file you'le notice the network tag. the network tag is used to network your containers together. This way all port mapping and port exposure can be removed from the containers. Only the port on whitch nginx listens is exposed to the public. This makes it really difficuild for hacker to access your containers.
 If this project is started up using the compose file make sure to update the connection string before running build command to: "Data Source = countrydb; Initial Catalog = CountriesGuid; User ID = sa; Password=Markus@2"
 Unfortunatly the migration commands needs to be run on the database container before the api container can startup properly as I do not outomate the database creation. This is easy to overcome: run compose up command, wait for database container to startup(api container will fail), update connection string to host,1433, then run the migration commands to update the database and then run the compose up command again. The database should be generated and the api container should startup.
 
