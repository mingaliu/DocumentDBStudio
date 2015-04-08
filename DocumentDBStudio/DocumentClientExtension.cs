namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    /// <summary>
    /// Provides a document client extension.
    /// </summary>
    internal static class DocumentClientExtension
    {
        /// <summary>
        /// Only used in PCV and CTL test, configure to switch between selflink and altlink
        /// </summary>
        /// <returns></returns>
        internal static string GetLink(this Resource resource, DocumentClient client)
        {
            return resource.SelfLink;
        }

        #region Replace operation
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
            return client.ReplaceUserAsync(user, options);
        }
        #endregion

    }
}
