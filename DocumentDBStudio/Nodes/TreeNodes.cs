//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.DocumentDBStudio
{
    using Documents.Client;

    internal class AccountSettings
    {
        public string FriendlyName;
        public string MasterKey;
        public ConnectionMode ConnectionMode;
        public Protocol Protocol;
        public bool IsNameBased;
        public bool EnableEndpointDiscovery;
        /// <summary>
        /// Specifies whether this account settings should be persisted locally  
        /// </summary>
        public bool IsPersistedLocally;
    }

    public class CommandContext
    {
        public bool IsDelete;
        public bool IsFeed;
        public bool IsCreateTrigger;
        public bool HasContinuation;
        public bool QueryStarted;
        public CommandContext()
        {
            IsDelete = false;
            IsFeed = false;
            HasContinuation = false;
            QueryStarted = false;
            IsCreateTrigger = false;
        }
    }

    enum ResourceType
    {
        DatabaseAccount,
        Database,
        DocumentCollection,
        Document,
        User,
        StoredProcedure,
        UserDefinedFunction,
        Trigger,
        Permission,
        Attachment,
        Conflict,
        Offer
    }

    enum OperationType
    {
        Create,
        Replace,
        Read,
        Delete,
        Query,
        Execute,
    }

    enum OfferType
    {
        S1,
        S2,
        S3,
        StandardSingle,
        StandardElastic
    }
}