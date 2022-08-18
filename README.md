# CountryApi
Gebou in .net 6 <br>
Eerste keer wat ek .net 6 gebruik so dit het meer tyd gevet as wat ek gedink het. Ek het wel geleentheid gevat om om die raamwerk beter te leer ken <br>
My ervaring met Entity framework as databasis bestuur bring gereeld die probleem van 'self verwanskap' voor wat lui na eindloose loops, en met die simplistiese databasis van die taak het ek geleentheid gevat om nuwe oplossings te probeer. <br>

# databasis
### classs wat net die databasis tafels direk voorstel

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

### classe wat die tafels se verwanskap foorstel

<code>

    [Table("Country")]
    public class CountryTable : Table.Country
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
    public class CurrencyTable : Table.Currency
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

### classe wat terug gestuur word client to van API

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

## Beskruiwing

Die klasse is so opgestel dat ek enige verwanskappe kan vermy wat dalk na n self vewanskappe kan lui. Die generise 'Currency' en 'Country' klasse se 'Properties' stem direk ooreen met die 'Fields' van die databasis. Die volgende klasse genoemd
'CurrencyTable' en 'CountryTable' behou die verwanskap tussen die twee tafels. Die klasse word vermy as ek data terug stuur na die klient. Ek noem hulle spesifiek met 'Table' agterna omdat die klasse gebruik work deer Entity framwork om die databasis en sy verwantskappe te genereer. <br>
Laastens is die 'Transfer' klasse. Hulle het Kinder klasse maar omdat ek gebruik maak van die generiese 'Country' en 'Currency' klasse is daar geen opvolgende kinder klasse teenwoordig nie. Hinheretance word dan gebruik om om die generise klasse se 'properties' in die twee funksie spesifieke knlasse in te bring.

# Eindpunte
Ek het die volle CRUD gebou met die gedagte om te sien hoe .net6 saam Entity Framework die Databasis struktuut hanteer en alhowel daar baie kode is, is meeste van dit ge genereer deer entity framwork.
 ## api/Countries
 1. get // All
 2. get(guid Id) // ById
 3. post(Country object) // Create
 4. put // Edit
 5. get(string predicate) // byPredicate: Enige oorstemming met die predicate en aplha2, alpha3, of numeric van n country wat op die databasis gestoor is word terug gestuur deur die API.

Country se Put en Post hanteer al hull kind objekte


## api/Currencies
 1. get // All
 2. get(guid Id) // ById
 3. post(Country object) // Create
 4. put // Edit
 5. get(string predicate) // byPredicate: all die countries met die currency wat se naam ooreen stem met predicate word terug gestuur

## Swagger/openApi
Die voledige API beskywing met voorbeel request in json formaat voleerd met a interface word ge genereer as https://<host>/swagger URL besoek word.<br>
Die json dokument kan dan in Postman ge importeer word en n voleedige colleksie van versokke wat elke eindpunt in die api toets word ge genereer. Met bietjie manipulasie is dit maklik om a postman colleksie the maak wat automatiese opvolgende versoeke na die API uitvoer en so die integrasie toetsing automatiseer.  

# Docker

> docker build -t reponame/reponame/imagename pathToDockefile
> docker build -t reponame/containername .

Die tweede voorbeeld werk as die termenal op die projek se path is.<br>
-t benoem die image wat in doe container hardloop. (reponame/imagename)Word na werwys in die docker compose file



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
<code>

</code>

## docker-compose
> docker-compose up
Die docker compose up command werk ook net as terminaal se path n docker-compose.yml file bevat.

<code>

    version: '3.2'

    services:

      proxy:
        container_name: proxy
        image: nginx:latest
        networks:
        - apinetwork
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
        image: reponame/imagename       // verwys na die reponame en imagename van die docker build command
        environment:
        - ASPNETCORE_ENVIRONMENT=Development
        expose:
        - 80
        ports:
        - "81:80"
        depends_on:
        - countryapi
        networks:
        - apinetwork
        depends_on:
        - countrydb

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
        - apinetwork
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
      apinetwork:

</code>

## Nginx

> sudo -H ./letsencrypt-auto certonly --standalone -d example.com -d www.example.com

Die boonste command moet gehardloop word op die host om die nodige ssl sertifikate te genereer. Met die volume mapping in die nginx container trek docker die sertifikate oor van die host af na di container toe. <br>
Die onderste file word ook met volume mapping van die host af no die nginx container to getrek. Die nginx container gebruik die config file om die versoek te verwys na die beskikbare ssl port toe. 

<code>

events {
    worker_connections 1024;
}

http {
        client_max_body_size 80000M;
    server {
        server_name <serverUrl>;

        location / {
        return 301 https://$host$request_uri;
        }

    }
    server {
        server_name <serverUrl>;
        location / {
        proxy_pass http://countryapi:80/;           // container name in compose file en die exposed port
        }
        listen 80;
        listen 443 ssl;
        ssl_certificate /etc/letsencrypt/live/<domain>.com/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/<domain>.com/privkey.pem;
        include /etc/letsencrypt/options-ssl-nginx.conf;
    }
}

</code>

## Volumes

Die volume mapping word gebruik om data te persist as die container ge-restart moet word of net op failure. Databsis data word na die host to gemap in die countryapi container en dan word die ssl en nginx config files van die host af na die proxy container toe gemap.<br> 

## Unit testing

Ongelulig het ek nie kaans gehad om enige Unit tests te skryf nie. Die UI demo het langer geneem as wat ek verwag het.<br>
Ek het ook meer ge-fokus op integrasie toeste aangesien die projek nie komplekse kelkelasies bevat nie en meer integrasie gefokus is. 

