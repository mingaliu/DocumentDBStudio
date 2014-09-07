Azure DocumentDB Studio
================

A client management/viewer for Microsoft Azure DocumentDB.

Currently it supports: 
<br/> 1. Easyly browse DocumentDB resources and learn DocumentDB resource model.
<br/> 2. Issue Insert, Replace, Delete, Read (CRUD) operations on every DocumentDB resource and resource feed. 
<br/> 3. Issue SQL/UDF query. Execute Javascript storedprocedure. 
<br/> 4. Inspect heads (for quota, usage, RG charge etc) for every operation.

To start:
<br />   1. Add your account from File|Add account. You can provision and get your account endpoint and masterkey from  <a href="https://portal.azure.com">https://portal.azure.com</a>
<br /> 2. You can start navigating the DocumentDB resource model in the left side treeview panel. 
<br /> 3. Right click any resource feed or item for supported CRUD or query or stored procedure operation.
<br />
    The tool run on Windows and requires .Net Framework 4.0 installed on your machine.<br />
    
I welcome everyone to contribue to this project.

Here is some ideas to do next:
<br /> 1. Another tab for RequestOptions (for pre/post trigger,  sessionToken etc).
<br /> 2. Richer support for editor (Syntax highlighting for SQL/Javascript, easy grid based editing for JSON).
<br /> 3. Some small update including: Paging for documents, Add option to refresh resource itself.
<br /> 4. UI refinement.



