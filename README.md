The starting data necessary for this project can be found in the repository under \wwwroot\uploads\. 
Operations.txt contains a list of actions that were performed by the vehicle:
- left 90 means the vehicle turned left by 90 degrees.
- forward 100 means that the vehicle moved forward 100 meters.
- picture PIA04191_orig.jpg means that it took a picture and recorded it in the file named PIA04191_orig.jpg.
- marker B means that a positional marker was placed label B.

You are given a demo Cosmos DB account & the tasks are as follows:
1. Write code in any language to upload the data into a collection in the Cosmos DB account.  Make any design choices you deem appropriate so that all the data can be stored in Cosmos DB.
2. Write code in any language, that uses the data stored in Cosmos DB, to find out the distance **travelled** in metres between markers A and C.
3. Similarly write something to find out the distance in metres between markers A and C.
