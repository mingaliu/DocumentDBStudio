using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Azure.DocumentDBStudio.Helpers;
using Microsoft.Azure.DocumentDBStudio.Providers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio
{
    class ResourceNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();
        private readonly ResourceType _resourceType = 0;
        private readonly string _databaseId;
        private readonly string _documentCollectionId;

        public ResourceNode(
            DocumentClient client, 
            dynamic document, 
            ResourceType resoureType, 
            PartitionKeyDefinition partitionKey = null, 
            string nodeText = null,
            string dataBaseId = null,
            string documentCollectionId = null
        )
        {
            _databaseId = dataBaseId;
            _documentCollectionId = documentCollectionId;
            _resourceType = resoureType;
            var docAsResource = (document as Resource);
            var isDocument = _resourceType == ResourceType.Document;
            var isOffer = _resourceType == ResourceType.Offer;
            var isConflict = _resourceType == ResourceType.Conflict;
            var isPermission = _resourceType == ResourceType.Permission;
            var isAttachment = _resourceType == ResourceType.Attachment;
            var isStoredProcedure = _resourceType == ResourceType.StoredProcedure;
            var isTrigger = _resourceType == ResourceType.Trigger;
            var isUserDefinedFunction = _resourceType == ResourceType.UserDefinedFunction;
            var isUser = _resourceType == ResourceType.User;

            if (isDocument)
            {
                var prefix = string.Empty;
                if (partitionKey != null)
                {
                    if (partitionKey.Paths.Count > 0)
                    {
                        var path = partitionKey.Paths[0];
                        prefix = document.GetPropertyValue<string>(path.Substring(1));
                        prefix = prefix + "_";
                    }
                }
                Text = string.IsNullOrWhiteSpace(nodeText) 
                    ? prefix + docAsResource.Id 
                    : prefix + nodeText;
            }
            else if (isOffer)
            {
                string version = document.GetPropertyValue<string>("offerVersion");
                if (string.IsNullOrEmpty(version))
                {
                    var offer = document as Offer;
                    Text = string.Format("{0}_{1}", offer.OfferType, offer.GetPropertyValue<string>("offerResourceId"));
                }
                else
                {
                    var offer = document as OfferV2;
                    Text = string.Format("{0}_{1}", offer.Content.OfferThroughput, offer.GetPropertyValue<string>("offerResourceId"));
                }
            }
            else
            {
                Text = docAsResource.Id;
            }

            Tag = document;
            _client = client;

            AddMenuItem(string.Format("Read {0}", _resourceType), myMenuItemRead_Click);

            if (!isConflict && !isOffer)
            {
                AddMenuItem(string.Format("Replace {0}", _resourceType), myMenuItemUpdate_Click);
            }
            if (!isOffer)
            {
                AddMenuItem(string.Format("Delete {0}", _resourceType), myMenuItemDelete_Click);
            }

            if (!isConflict && !isOffer)
            {
                _contextMenu.MenuItems.Add("-");

                AddMenuItem("Copy id to clipboard", myMenuItemCopyIdToClipBoard_Click);
                AddMenuItem(string.Format("Copy {0} to clipboard", _resourceType), myMenuItemCopyToClipBoard_Click);
                AddMenuItem(string.Format("Copy {0} to clipboard with new id", _resourceType), myMenuItemCopyToClipBoardWithNewId_Click);
            }

            if (isPermission)
            {
                ImageKey = "Permission";
                SelectedImageKey = "Permission";
            }
            else if (isAttachment)
            {
                ImageKey = "Attachment";
                SelectedImageKey = "Attachment";

                AddMenuItem("Download media", myMenuItemDownloadMedia_Click);
                AddMenuItem("Render media", myMenuItemRenderMedia_Click);
            }
            else if (isStoredProcedure || isTrigger || isUserDefinedFunction)
            {
                ImageKey = "Javascript";
                SelectedImageKey = "Javascript";
                if (isStoredProcedure)
                {
                    AddMenuItem(string.Format("Execute {0}", _resourceType), myMenuItemExecuteStoredProcedure_Click);
                }
            }
            else if (isUser)
            {
                ImageKey = "User";
                SelectedImageKey = "User";

                Nodes.Add(new PermissionNode(_client));
            }
            else if (isDocument)
            {
                Nodes.Add(new TreeNode("Fake"));

                _contextMenu.MenuItems.Add("-");

                AddMenuItem("Create attachment", myMenuItemCreateAttachment_Click);
                AddMenuItem("Create attachment from file", myMenuItemAttachmentFromFile_Click);
            }
            else if (isConflict)
            {
                ImageKey = "Conflict";
                SelectedImageKey = "Conflict";
            }
            else if (isOffer)
            {
                ImageKey = "Offer";
                SelectedImageKey = "Offer";
            }
        }

        private void AddMenuItem(string menuItemText, EventHandler eventHandler)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            _contextMenu.MenuItems.Add(menuItem);
        }

        void myMenuItemCopyIdToClipBoard_Click(object sender, EventArgs eventArg)
        {
            try
            {
                string clipBoardContent;
                switch (_resourceType)
                {
                    case ResourceType.StoredProcedure:
                        clipBoardContent = (Tag as StoredProcedure).Id;
                        break;
                    case ResourceType.Trigger:
                        clipBoardContent = (Tag as Trigger).Id;
                        break;
                    case ResourceType.UserDefinedFunction:
                        clipBoardContent = (Tag as UserDefinedFunction).Id;
                        break;
                    default:
                        dynamic obj = JObject.Parse(Tag.ToString());
                        clipBoardContent = obj.id;
                        break;
                }
                Clipboard.SetText(clipBoardContent);
            }
            catch { }
        }
        
        void myMenuItemCopyToClipBoard_Click(object sender, EventArgs eventArg)
        {
            var clipBoardContent = GetCurrentObjectContents();
            Clipboard.SetText(clipBoardContent);
        }

        void myMenuItemCopyToClipBoardWithNewId_Click(object sender, EventArgs eventArg)
        {
            var clipBoardContent = GetCurrentObjectContents();
            clipBoardContent = DocumentHelper.AssignNewIdToDocument(clipBoardContent);
            Clipboard.SetText(clipBoardContent);
        }

        private string GetCurrentObjectContents()
        {
            string content;
            switch (_resourceType)
            {
                case ResourceType.StoredProcedure:
                    content = (Tag as StoredProcedure).Body;
                    break;
                case ResourceType.Trigger:
                    content = (Tag as Trigger).Body;
                    break;
                case ResourceType.UserDefinedFunction:
                    content = (Tag as UserDefinedFunction).Body;
                    break;
                default:
                    content = Tag.ToString();
                    break;
            }
            content = DocumentHelper.RemoveInternalDocumentValues(content);
            return content;
        }

        void myMenuItemUpdate_Click(object sender, EventArgs e)
        {
            switch (_resourceType)
            {
                case ResourceType.StoredProcedure:
                    SetCrudContext(this, OperationType.Replace, _resourceType, (Tag as StoredProcedure).Body, ReplaceResourceAsync);
                    break;
                case ResourceType.Trigger:
                    SetCrudContext(this, OperationType.Replace, _resourceType, (Tag as Trigger).Body, ReplaceResourceAsync);
                    break;
                case ResourceType.UserDefinedFunction:
                    SetCrudContext(this, OperationType.Replace, _resourceType, (Tag as UserDefinedFunction).Body, ReplaceResourceAsync);
                    break;
                default:
                    var tag = Tag.ToString();
                    tag = DocumentHelper.RemoveInternalDocumentValues(tag);
                    SetCrudContext(this, OperationType.Replace, _resourceType, tag, ReplaceResourceAsync);
                    break;
            }
        }

        async void myMenuItemRead_Click(object sender, EventArgs eventArg)
        {
            try
            {
                switch (_resourceType)
                {
                    case ResourceType.Offer:
                    {
                        ResourceResponse<Offer> rr;
                        using (PerfStatus.Start("ReadOffer"))
                        {
                            rr = await _client.ReadOfferAsync(((Resource)Tag).SelfLink);
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.Document:
                    {
                        var document = (Document)Tag;
                        var collection = (DocumentCollection)Parent.Tag;

                        var requestOptions = GetRequestOptions();
                        if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                        {
                            requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                        }

                        ResourceResponse<Document> rr;
                        using (PerfStatus.Start("ReadDocument"))
                        {
                            rr = await _client.ReadDocumentAsync(document.GetLink(_client), requestOptions);
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);
                        json = DocumentHelper.RemoveInternalDocumentValues(json);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.Conflict:
                    {
                        ResourceResponse<Conflict> rr;
                        using (PerfStatus.Start("ReadConflict"))
                        {
                            rr = await _client.ReadConflictAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.Attachment:
                    {
                        ResourceResponse<Attachment> rr;
                        var document = ((Document)Tag);
                        var collection = ((DocumentCollection)Parent.Tag);

                        var requestOptions = GetRequestOptions();
                        if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                        {
                            requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                        }

                        using (PerfStatus.Start("ReadAttachment"))
                        {
                            rr = await _client.ReadAttachmentAsync(document.GetLink(_client), requestOptions);
                        }

                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.User:
                    {
                        ResourceResponse<User> rr;
                        using (PerfStatus.Start("ReadUser"))
                        {
                            rr = await _client.ReadUserAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.Permission:
                    {
                        ResourceResponse<Permission> rr;
                        using (PerfStatus.Start("ReadPermission"))
                        {
                            rr = await _client.ReadPermissionAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.StoredProcedure:
                    {
                        ResourceResponse<StoredProcedure> rr;
                        using (PerfStatus.Start("ReadStoredProcedure"))
                        {
                            rr = await _client.ReadStoredProcedureAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.Trigger:
                    {
                        ResourceResponse<Trigger> rr;
                        using (PerfStatus.Start("ReadTrigger"))
                        {
                            rr = await _client.ReadTriggerAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    case ResourceType.UserDefinedFunction:
                    {
                        ResourceResponse<UserDefinedFunction> rr;
                        using (PerfStatus.Start("ReadUDF"))
                        {
                            rr = await _client.ReadUserDefinedFunctionAsync(((Resource)Tag).GetLink(_client), GetRequestOptions());
                        }
                        // set the result window
                        var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                    }
                        break;
                    default:
                        throw new ArgumentException("Unsupported resource type " + _resourceType);
                }

            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

        void myMenuItemCreateAttachment_Click(object sender, EventArgs e)
        {
            var attachment = new Attachment
            {
                Id = "Here is your attachment Id",
                ContentType = "application-content-type",
                MediaLink = "internal link or Azure blob or Amazon S3 link"
            };

            var bodytext = attachment.ToString();
            SetCrudContext(this, OperationType.Create, _resourceType, bodytext, CreateAttachmentAsync);
        }

        async void myMenuItemRenderMedia_Click(object sender, EventArgs eventArg)
        {
            var guidFileName = Guid.NewGuid().ToString();
            string fileName;

            // let's guess the contentype.
            var attachment = Tag as Attachment;
            if (string.Compare(attachment.ContentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // get the extension from attachment.Id
                var index = attachment.Id.LastIndexOf('.');
                fileName = guidFileName + attachment.Id.Substring(index);
            }
            else if (attachment.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                // treat as image.
                fileName = guidFileName + ".gif";
            }
            else
            {
                fileName = guidFileName + ".txt";
            }

            fileName = Path.Combine(SystemInfoProvider.LocalApplicationDataPath, fileName);
            try
            {
                MediaResponse rr;
                using (PerfStatus.Start("DownloadMedia"))
                {
                    rr = await _client.ReadMediaAsync(attachment.MediaLink);
                }
                using (var fileStream = File.Create(fileName))
                {
                    rr.Media.CopyTo(fileStream);
                }

                SetResultInBrowser(null, "It is saved to " + fileName, true);
                Program.GetMain().RenderFile(fileName);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

  
        async void myMenuItemDownloadMedia_Click(object sender, EventArgs eventArg)
        {
            var attachment = Tag as Attachment;

            // Get the filenanme from attachment.Id
            var index = attachment.Id.LastIndexOf('\\');
            var fileName = attachment.Id;
            if (index > 0)
                fileName = fileName.Substring(index + 1);

            var ofd = new SaveFileDialog {FileName = fileName};
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var saveFile = ofd.FileName;
                Program.GetMain().SetLoadingState();

                try
                {
                    MediaResponse rr;
                    using (PerfStatus.Start("DownloadMedia"))
                    {
                        rr = await _client.ReadMediaAsync(attachment.MediaLink);
                    }
                    using (var fileStream = File.Create(saveFile))
                    {
                        rr.Media.CopyTo(fileStream);
                    }
                    SetResultInBrowser(null, "It is saved to " + saveFile, true);
                }
                catch (Exception e)
                {
                    SetResultInBrowser(null, e.ToString(), true);
                }
            }
        }

        async void myMenuItemAttachmentFromFile_Click(object sender, EventArgs eventArg)
        {
            var ofd = new OpenFileDialog();
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var filename = ofd.FileName;
                // 
                // todo: present the dialog for Slug name and Content type
                // 
                Program.GetMain().SetLoadingState();

                try
                {

                    using (var stream = new FileStream(filename,
                        FileMode.Open, FileAccess.Read))
                    {
                        var mediaOptions = new MediaOptions()
                        {
                            ContentType = "application/octet-stream",
                            Slug = Path.GetFileName(ofd.FileName)
                        };

                        ResourceResponse<Attachment> rr;

                        var document = ((Document)Tag);
                        var collection = ((DocumentCollection)Parent.Tag);

                        var requestOptions = GetRequestOptions();
                        if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                        {
                            requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                        }

                        using (PerfStatus.Start("CreateAttachment"))
                        {
                            rr = await _client.CreateAttachmentAsync((Tag as Document).SelfLink + "/attachments",
                                      stream, mediaOptions, requestOptions);
                        }

                        var json = rr.Resource.ToString();

                        SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                        Nodes.Add(new ResourceNode(_client, rr.Resource, ResourceType.Attachment));
                    }
                }
                catch (Exception e)
                {
                    SetResultInBrowser(null, e.ToString(), true);
                }
            }
        }

        void myMenuItemExecuteStoredProcedure_Click(object sender, EventArgs e)
        {
            SetCrudContext(this, OperationType.Execute, _resourceType,
                "Here is the input parameters to the storedProcedure. Input each parameter as one line without quotation mark.", ExecuteStoredProcedureAsync);
        }

        void myMenuItemDelete_Click(object sender, EventArgs e)
        {
            var bodytext = Tag.ToString();
            var context = new CommandContext {IsDelete = true};
            SetCrudContext(this, OperationType.Delete, _resourceType, bodytext, DeleteResourceAsync, context);
        }

        async void CreateAttachmentAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var text = resource as string;
                var attachment = (Attachment)JsonConvert.DeserializeObject(text, typeof(Attachment));

                ResourceResponse<Attachment> rr;
                using (PerfStatus.Start("CreateAttachment"))
                {
                    rr = await _client.CreateAttachmentAsync((Tag as Resource).GetLink(_client),
                                        attachment, requestOptions);
                }
                var json = rr.Resource.ToString();

                SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                Nodes.Add(new ResourceNode(_client, rr.Resource, ResourceType.Attachment));
            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void ExecuteStoredProcedureAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var text = resource as string;
                var inputParamters = new List<string>();
                if (!string.IsNullOrEmpty(text))
                {
                    using (var sr = new StringReader(text))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                inputParamters.Add(line);
                            }
                        }//while
                    }//usi
                }
                var dynamicInputParams = new dynamic[inputParamters.Count];
                for (var i = 0; i < inputParamters.Count; i++)
                {
                    var inputParamter = inputParamters[i];
                    var jTokenParam = JToken.Parse(inputParamter);
                    var dynamicParam = Helper.ConvertJTokenToDynamic(jTokenParam);
                    dynamicInputParams[i] = dynamicParam;
                }

                StoredProcedureResponse<dynamic> rr;
                using (PerfStatus.Start("ExecuateStoredProcedure"))
                {
                    rr = await _client.ExecuteStoredProcedureAsync<dynamic>((Tag as Resource).GetLink(_client),
                                      dynamicInputParams);
                }
                string executeResult = rr.Response.ToString();

                SetResultInBrowser(null, executeResult, true, rr.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void ReplaceResourceAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string json = null;
                if (_resourceType == ResourceType.Document)
                {
                    var text = resource as string;
                    var doc = (Document)JsonConvert.DeserializeObject(text, typeof(Document));
                    var tagAsDoc = (Tag as Document);
                    doc.SetReflectedPropertyValue("AltLink", tagAsDoc.GetAltLink());
                    ResourceResponse<Document> rr;
                    var hostName = _client.ServiceEndpoint.Host;
                    using (PerfStatus.Start("ReplaceDocument"))
                    {
                        rr = await _client.ReplaceDocumentAsync(doc.GetLink(_client), doc, requestOptions);
                    }
                    json = rr.Resource.ToString();

                    Tag = rr.Resource;

                    Text = DocumentHelper.GetDisplayText(rr.Resource, hostName, _documentCollectionId, _databaseId);
                    // set the result window
                    SetResultInBrowser(DocumentHelper.RemoveInternalDocumentValues(json), null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.StoredProcedure)
                {
                    var input = resource as StoredProcedure;
                    var sp = Tag as StoredProcedure;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<StoredProcedure> rr;
                    using (PerfStatus.Start("ReplaceStoredProcedure"))
                    {
                        rr = await _client.ReplaceStoredProcedureExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.User)
                {
                    var text = resource as string;
                    var sp = (User)JsonConvert.DeserializeObject(text, typeof(User));
                    sp.SetReflectedPropertyValue("AltLink", (Tag as User).GetAltLink());
                    ResourceResponse<User> rr;
                    using (PerfStatus.Start("ReplaceUser"))
                    {
                        rr = await _client.ReplaceUserExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Trigger)
                {
                    var input = resource as Trigger;
                    var sp = Tag as Trigger;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<Trigger> rr;
                    using (PerfStatus.Start("ReplaceTrigger"))
                    {
                        rr = await _client.ReplaceTriggerExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.UserDefinedFunction)
                {
                    var input = resource as UserDefinedFunction;
                    var sp = Tag as UserDefinedFunction;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<UserDefinedFunction> rr;
                    using (PerfStatus.Start("ReplaceUDF"))
                    {
                        rr = await _client.ReplaceUserDefinedFunctionExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Permission)
                {
                    var text = resource as string;
                    var sp = JsonSerializable.LoadFrom<Permission>(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                    sp.SetReflectedPropertyValue("AltLink", (Tag as Permission).GetAltLink());
                    ResourceResponse<Permission> rr;
                    using (PerfStatus.Start("ReplacePermission"))
                    {
                        rr = await _client.ReplacePermissionExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Attachment)
                {
                    var text = resource as string;
                    var sp = (Attachment)JsonConvert.DeserializeObject(text, typeof(Attachment));
                    sp.SetReflectedPropertyValue("AltLink", (Tag as Attachment).GetAltLink());
                    ResourceResponse<Attachment> rr;

                    var document = ((Document)Parent.Tag);
                    var collection = ((DocumentCollection)Parent.Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("ReplaceAttachment"))
                    {
                        rr = await _client.ReplaceAttachmentExAsync(sp, requestOptions);
                    }

                    json = rr.Resource.ToString();
                    Tag = rr.Resource;
                    Text = rr.Resource.Id;
                    // set the result window
                    SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void DeleteResourceAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                if (_resourceType == ResourceType.Document)
                {
                    var doc = (Document)Tag;
                    ResourceResponse<Document> rr;
                    var collection = ((DocumentCollection)Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(doc, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("DeleteDocument"))
                    {
                        rr = await _client.DeleteDocumentAsync(doc.GetLink(_client), requestOptions);
                    }

                    SetResultInBrowser(null, "Delete Document succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.StoredProcedure)
                {
                    var sp = (StoredProcedure)Tag;
                    ResourceResponse<StoredProcedure> rr;
                    using (PerfStatus.Start("DeleteStoredProcedure"))
                    {
                        rr = await _client.DeleteStoredProcedureAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete StoredProcedure succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.User)
                {
                    var sp = (User)Tag;
                    ResourceResponse<User> rr;
                    using (PerfStatus.Start("DeleteUser"))
                    {
                        rr = await _client.DeleteUserAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete User succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Trigger)
                {
                    var sp = (Trigger)Tag;
                    ResourceResponse<Trigger> rr;
                    using (PerfStatus.Start("DeleteTrigger"))
                    {
                        rr = await _client.DeleteTriggerAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete Trigger succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.UserDefinedFunction)
                {
                    var sp = (UserDefinedFunction)Tag;
                    ResourceResponse<UserDefinedFunction> rr;
                    using (PerfStatus.Start("DeleteUDF"))
                    {
                        rr = await _client.DeleteUserDefinedFunctionAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete UserDefinedFunction succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Permission)
                {
                    var sp = (Permission)Tag;
                    ResourceResponse<Permission> rr;
                    using (PerfStatus.Start("DeletePermission"))
                    {
                        rr = await _client.DeletePermissionAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete Permission succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Attachment)
                {
                    var sp = (Attachment)Tag;
                    ResourceResponse<Attachment> rr;

                    var document = ((Document)Parent.Tag);
                    var collection = ((DocumentCollection)Parent.Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("DeleteAttachment"))
                    {
                        rr = await _client.DeleteAttachmentAsync(sp.GetLink(_client), requestOptions);
                    }

                    SetResultInBrowser(null, "Delete Attachment succeed!", false, rr.ResponseHeaders);
                }
                else if (_resourceType == ResourceType.Conflict)
                {
                    var sp = (Conflict)Tag;
                    ResourceResponse<Conflict> rr;
                    using (PerfStatus.Start("DeleteConlict"))
                    {
                        rr = await _client.DeleteConflictAsync(sp.GetLink(_client), requestOptions);
                    }
                    SetResultInBrowser(null, "Delete Conflict succeed!", false, rr.ResponseHeaders);
                }
                // Remove the node.
                Remove();

            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            _contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || IsFirstTime)
            {
                IsFirstTime = false;
                Nodes.Clear();

                if (_resourceType == ResourceType.User)
                {
                    Nodes.Add(new PermissionNode(_client));
                }
                else if (_resourceType == ResourceType.Document)
                {
                    FillWithChildren();
                }

            }
        }

        public string GetBody()
        {
            string body = null;
            if (_resourceType == ResourceType.StoredProcedure)
            {
                body = "\nThe storedprocedure Javascript function: \n\n" + (Tag as StoredProcedure).Body;
            }
            else if (_resourceType == ResourceType.Trigger)
            {
                body = "\nThe trigger Javascript function: \n\n" + (Tag as Trigger).Body;
            }
            else if (_resourceType == ResourceType.UserDefinedFunction)
            {
                body = "\nThe stored Javascript function: \n\n" + (Tag as UserDefinedFunction).Body;
            }
            return body;
        }

        private void SetResultInBrowser(string json, string text, bool executeButtonEnabled, NameValueCollection responseHeaders = null)
        {
            Program.GetMain().SetResultInBrowser(json, text, executeButtonEnabled, responseHeaders);
        }

        private RequestOptions GetRequestOptions()
        {
            return Program.GetMain().GetRequestOptions();
        }

        private void SetCrudContext(TreeNode node, OperationType operation, ResourceType resourceType, string bodytext,
            Action<object, RequestOptions> func, CommandContext commandContext = null)
        {
            Program.GetMain().SetCrudContext(node, operation, resourceType, bodytext, func, commandContext);
        }

        public void FillWithChildren()
        {
            try
            {
                var document = ((Document)Tag);
                var collection = ((DocumentCollection)Parent.Tag);

                var options = new FeedOptions();
                if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                {
                    options.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                }

                FeedResponse<Attachment> attachments;
                using (PerfStatus.Start("ReadAttachmentFeed"))
                {
                    attachments = _client.ReadAttachmentFeedAsync(document.GetLink(_client), options).Result;
                }

                foreach (var attachment in attachments)
                {
                    var node = new ResourceNode(_client, attachment, ResourceType.Attachment);
                    Nodes.Add(node);
                }

                Program.GetMain().SetResponseHeaders(attachments.ResponseHeaders);
            }
            catch (AggregateException e)
            {
                SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }
}
