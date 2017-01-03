namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Documents;
    using Documents.Client;
    using System.Reflection;
    using System.Globalization;

    public static class ReflectionHelper
    {
        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo propInfo = null;
            do
            {
                propInfo = type.GetProperty(propertyName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            while (propInfo == null && type != null);
            return propInfo;
        }

        public static object GetReflectedPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                  string.Format(CultureInfo.InvariantCulture, "Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            return propInfo.GetValue(obj, null);
        }

        public static void SetReflectedPropertyValue(this object obj, string propertyName, object val)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                  string.Format(CultureInfo.InvariantCulture, "Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            propInfo.SetValue(obj, val, null);
        }
    }

    /// <summary>
    /// Provides a document client extension.
    /// </summary>
    internal static class DocumentClientExtension
    {
        static Dictionary<string, bool> NameRoutingMap = new Dictionary<string, bool>();

        internal static void AddOrUpdate(string endpoint, bool IsNameBased)
        {
            if (NameRoutingMap.ContainsKey(endpoint))
            {
                NameRoutingMap.Remove(endpoint);
            }
            NameRoutingMap.Add(endpoint, IsNameBased);
        }

        /// <summary>
        /// Only used in PCV and CTL test, configure to switch between selflink and altlink
        /// </summary>
        /// <returns></returns>
        internal static string GetLink(this Resource resource, DocumentClient client)
        {
            if (NameRoutingMap[client.ServiceEndpoint.Host])
            {
                return GetAltLink(resource);
            }
            else
            {
                return resource.SelfLink;
            }
        }

        public static string GetAltLink(this Resource resource)
        {
            try
            {
                string altlink = (string)resource.GetReflectedPropertyValue("AltLink");
                return altlink;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// For Replace operation, swap the link of selflink, 
        /// </summary>
        /// <param name="resource"></param>
        private static void SwapLinkIfNeeded(DocumentClient client, Resource resource)
        {
            if (NameRoutingMap[client.ServiceEndpoint.Host])
            {
                resource.SetReflectedPropertyValue("SelfLink", resource.GetAltLink());
            }
        }

        #region Replace operation
        /// <summary>
        /// Replaces an DocumentCollection as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="attachmentsUri">the updated attachment.</param>
        /// <param name="attachment">the DocumentCollection resource.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionExAsync(this DocumentClient client, DocumentCollection collection, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, collection);
            return client.ReplaceDocumentCollectionAsync(collection, options);
        }

        /// <summary>
        /// Replaces an attachment as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="attachmentsUri">the updated attachment.</param>
        /// <param name="attachment">the attachment resource.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<Attachment>> ReplaceAttachmentExAsync(this DocumentClient client, Attachment attachment, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, attachment);
            return client.ReplaceAttachmentAsync(attachment, options);
        }

        /// <summary>
        /// Replace the specified stored procedure.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="storedProcedureUri">the self-link for the attachment.</param>
        /// <param name="storedProcedure">the updated stored procedure.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureExAsync(this DocumentClient client, StoredProcedure storedProcedure, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, storedProcedure);
            return client.ReplaceStoredProcedureAsync(storedProcedure, options);
        }

        /// <summary>
        /// Replaces a trigger as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="triggerUri">the self-link for the attachment.</param>
        /// <param name="trigger">the updated trigger.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<Trigger>> ReplaceTriggerExAsync(this DocumentClient client, Trigger trigger, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, trigger);
            return client.ReplaceTriggerAsync(trigger, options);
        }

        /// <summary>
        /// Replaces a user defined function as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="userDefinedFunctionUri">the self-link for the attachment.</param>
        /// <param name="function">the updated user defined function.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionExAsync(this DocumentClient client, UserDefinedFunction function, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, function);
            return client.ReplaceUserDefinedFunctionAsync(function, options);
        }

        /// <summary>
        /// Replaces a permission as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="permissionUri">the self-link for the attachment.</param>
        /// <param name="permission">the updated permission.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<Permission>> ReplacePermissionExAsync(this DocumentClient client, Permission permission, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, permission);
            return client.ReplacePermissionAsync(permission, options);
        }

        /// <summary>
        /// Replaces a user as an asynchronous operation.
        /// </summary>
        /// <param name="client">document client.</param>
        /// <param name="userUri">the self-link for the attachment.</param>
        /// <param name="user">the updated user.</param>
        /// <param name="options">the request options for the request.</param>
        /// <returns>The task object representing the service response for the asynchronous operation.</returns>
        public static Task<ResourceResponse<User>> ReplaceUserExAsync(this DocumentClient client, User user, RequestOptions options = null)
        {
            SwapLinkIfNeeded(client, user);
            return client.ReplaceUserAsync(user, options);
        }
        #endregion

    }
}
