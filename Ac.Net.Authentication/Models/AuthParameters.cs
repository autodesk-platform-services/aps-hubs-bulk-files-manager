using Autodesk.Authentication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ac.Net.Authentication.Models
{
    public class AuthParameters : INotifyPropertyChanged
    {
        private object _lock = new object();

        private string refreshToken;

        /// <summary>
        /// Default Constuctor
        /// </summary>
        public AuthParameters()
        {
            ClientId = "";
            ForgeCallback = "";
            Scope = new List<Scopes>();
            ClientId = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string ClientId { get; set; }

        public string ForgeCallback { get; set; }

        public bool IsImplicit { get; set; } = false;

        public string RefreshToken { get => refreshToken; set { lock (_lock) refreshToken = value; } }

        public List<Scopes> Scope { get; set; }

        public string Secret { get; set; }

        public bool IsValid()
        {
            //if (string.IsNullOrWhiteSpace(ClientId)) throw new NullReferenceException("Client ID is Invalid");  -- get error on initial load when clientId is null
            if (Scope.Count == 0) throw new NullReferenceException("Scope is Invalid");
            if (string.IsNullOrWhiteSpace(ForgeCallback)) throw new NullReferenceException("Authentication Callback is Invalid");
            if (IsImplicit && string.IsNullOrWhiteSpace(Secret)) throw new NullReferenceException("Secret is Invalid");
            return true;
        }
    }

}
