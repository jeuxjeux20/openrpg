﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenRPG {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   Classe de ressource fortement typée destinée, entre autres, à la recherche de chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ConnectionDefaults {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal ConnectionDefaults() {
        }
        
        /// <summary>
        ///    Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenRPG.ConnectionDefaults", typeof(ConnectionDefaults).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///    les recherches de ressources utilisant cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Recherche une chaîne localisée similaire à openrpg.
        /// </summary>
        public static string SQLDatabase {
            get {
                return ResourceManager.GetString("SQLDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Recherche une chaîne localisée similaire à localhost.
        /// </summary>
        public static string SQLHost {
            get {
                return ResourceManager.GetString("SQLHost", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Recherche une chaîne localisée similaire à pass.
        /// </summary>
        public static string SQLPass {
            get {
                return ResourceManager.GetString("SQLPass", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Recherche une chaîne localisée similaire à root.
        /// </summary>
        public static string SQLUsername {
            get {
                return ResourceManager.GetString("SQLUsername", resourceCulture);
            }
        }
    }
}
