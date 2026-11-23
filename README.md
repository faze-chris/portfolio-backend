Portfolio Backend API
Dit is de backend voor mijn portfolio. Het is gebouwd met ASP.NET Core Minimal API en Identity Framework. Voor de data base opslag wordt een PostgreSQL database gebruikt.

Stappenplan: Project opstarten
Volg de stappen om het project voor de eerste keer localhost te laten runnen:

Open de terminal in het project.

Typ het commando "dotnet build" in om te controleren of er geen typefouten in je code zitten.

Typ het commando "dotnet ef database update" in. Dit zorgt ervoor dat de lege database wordt gevuld met de juiste tabellen (zoals users, projecten en blogposts).

Typ het commando "dotnet run" in om de lokale server op te starten.

Open je browser en ga naar de lokale link die in de terminal verschijnt voeg /swagger toe aan de URL als dat nodig is. Hier kun je alle endpoints zien en testen.

Data toevoegen aan de database
Er zijn twee manieren waarop er data in de database komt:

Automatisch bij het opstarten (DbInitializer)
Zodra je de server start met "dotnet run", draait er een stukje code op de achtergrond. Deze code checkt of de database helemaal leeg is. Is dat zo? Dan maakt hij direct de rollen Admin en Gast aan, en voegt hij meteen een test-admin toe. Zo kun je altijd inloggen.

Handmatig via Swagger (POST requests)
Om zelf nieuwe projecten of blogposts toe te voegen, doe je het volgende in de browser via Swagger:

Ga naar de tab /auth/login endpoint en log in met de test-admin gegevens.

Na het inloggen herkent de server jou als Admin.

Ga naar de POST endpoint van bijvoorbeeld /api/projects.

Vul de JSON velden in (zoals Title, Description en ImageUrl).

Klik op Execute. De API pakt jouw data op en slaat deze veilig op in de PostgreSQL database.

Veelgebruikte Commando's
Hier is een lijst met commando's die veel worden gebruikt tijdens het programmeren:

"dotnet run": Start je lokale ASP.NET Core server op. Hiermee komt je backend online en kun je via de browser naar je Swagger UI navigeren om je endpoints te testen.

"dotnet build": Controleert je project op syntaxfouten zonder de server op te starten. Handig om na een aanpassing snel te checken of je ergens een puntkomma bent vergeten.

"dotnet ef migrations add ": Maakt een nieuw migratiebestand aan. Dit commando gebruik je direct nadat je een veld in je datamodel hebt toegevoegd, gewijzigd of verwijderd (bijvoorbeeld "dotnet ef migrations add AddImageUrl"). Zorg dat de server gestopt is voordat je dit typt.

"dotnet ef database update": Pusht de wijzigingen uit je migratie daadwerkelijk naar de PostgreSQL database. Dit zorgt ervoor dat de nieuwe tabellen of kolommen fysiek worden aangemaakt.

Testen Uitvoeren (xUnit)
Het project bevat integratietesten voor de CRUD-operaties (Create, Read, Update, Delete) en checkt ook direct of de beveiliging werkt. 

Typ: "dotnet test PortfolioSolution.slnx"