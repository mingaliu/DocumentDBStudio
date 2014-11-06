Azure DocumentDB Studio
================

A client management viewer/explorer for Microsoft Azure DocumentDB.

Currently it supports: 
<br/> 1. Easyly browse DocumentDB resources and learn DocumentDB resource model.
<br/> 2. Issue Insert, Replace, Delete, Read (CRUD), Query operations on every DocumentDB resource and resource feed. 
<br/> 3. Issue SQL/UDF query. Execute Javascript storedprocedure. Execute Trigger.
<br/> 4. Inspect heads (for quota, usage, RG charge etc) for every operation.
<br/> 5. Support RequestOptions(for pre/post trigger,  sessionToken, consistency model etc), FeedOptions(for paging, enableScanforQuery etc), IndexingPolicy (for indexingMode, indexingType, indexingPath etc).
<br/> 6. PrettyPrint the output JSON.
<br/> 7. Bulk import the JSON files. 

To start:
<br />   1. Add your account from File|Add account. You can provision and get your account endpoint and masterkey from  <a href="https://portal.azure.com">https://portal.azure.com</a>
<br /> 2. You can start navigating the DocumentDB resource model in the left side treeview panel. 
<br /> 3. Right click any resource feed or item for supported CRUD or query or stored procedure operation.
<br />
    The tool run on Windows and requires .Net Framework 4.0 installed on your machine.<br />
    
Future ideas to improve:
<br /> 1. Richer intellisense support for editor (Syntax highlighting for SQL/Javascript, easy grid based editing for JSON).

I welcome everyone to contribue to this project. Drop me a note (mingaliu@hotmail.com) if you have anyquestion.  





