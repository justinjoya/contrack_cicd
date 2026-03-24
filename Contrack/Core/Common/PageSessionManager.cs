using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public static class PageSessionManager
    {
        [Serializable]
        public class PageState
        {
            public object Filter { get; set; }
            public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
        }

        public static void SetFilter(string pageKey, object filter)
        {
            var states = SessionManager.UserPageStates;
            if (!states.ContainsKey(pageKey))
            {
                states[pageKey] = new PageState();
            }
            states[pageKey].Filter = filter;
            SessionManager.UserPageStates = states;
        }

        public static T GetFilter<T>(string pageKey) where T : new()
        {
            var states = SessionManager.UserPageStates;
            if (states.ContainsKey(pageKey) && states[pageKey].Filter != null)
            {
                try
                {
                    return (T)states[pageKey].Filter;
                }
                catch
                {
                    return new T();
                }
            }
            return new T();
        }

        public static void SetAttribute(string pageKey, string attributeKey, object value)
        {
            var states = SessionManager.UserPageStates;
            if (!states.ContainsKey(pageKey))
            {
                states[pageKey] = new PageState();
            }

            if (states[pageKey].Attributes.ContainsKey(attributeKey))
            {
                states[pageKey].Attributes[attributeKey] = value;
            }
            else
            {
                states[pageKey].Attributes.Add(attributeKey, value);
            }
            SessionManager.UserPageStates = states;
        }

        public static T GetAttribute<T>(string pageKey, string attributeKey, T defaultvalue)
        {
            var states = SessionManager.UserPageStates;
            if (states.ContainsKey(pageKey) && states[pageKey].Attributes.ContainsKey(attributeKey))
            {
                try
                {
                    return (T)states[pageKey].Attributes[attributeKey];
                }
                catch
                {
                    return defaultvalue;
                }
            }
            return defaultvalue;
        }
    }
}