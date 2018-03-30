using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BaseGameLogic.Networking
{
    [CustomEditor(typeof(BaseNetworkManager), true)]
    public class NetworkManagerCustomInspector : Editor
    {
        private BaseNetworkManager manager = null;
        private Type[] messageSenderTypes = null;
        private Type[] messageHandlerTypes = null;

        private GenericMenu messageSenderContextMenu = new GenericMenu();
        private GenericMenu messageHandlerContextMenu = new GenericMenu();

        private void OnEnable()
        {
            manager = target as BaseNetworkManager;

            messageSenderTypes = AssemblyExtension.GetDerivedTypes<BaseMessageSender>();
            messageHandlerTypes = AssemblyExtension.GetDerivedTypes<BaseMessageHandler>();

            GUIContent content = null;
            for (int i = 0; i < messageSenderTypes.Length; i++)
            {
                content = new GUIContent(messageSenderTypes[i].Name);
                messageSenderContextMenu.AddItem(content, false, AddMessageSender, i);

            }

            for (int i = 0; i < messageHandlerTypes.Length; i++)
            {
                content = new GUIContent(messageHandlerTypes[i].Name);
                messageHandlerContextMenu.AddItem(content, false, AddMessageHandler, i);
            }

            content = new GUIContent("Add all");
            messageHandlerContextMenu.AddItem(content, false, AddAllMessageHandler);

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add message sender"))
                {
                    messageSenderContextMenu.ShowAsContext();
                }

                if (GUILayout.Button("Add message handler"))
                {
                    messageHandlerContextMenu.ShowAsContext();
                }
            }
            GUILayout.EndHorizontal();
        }

        public void AddMessageHandler(object data)
        {
            int index = (int)data;
            Type type = messageHandlerTypes[index];
            manager.AddNewMessageHandler(type);

            Debug.Log(string.Format("{0} added.", type.Name));
        }

        public void AddMessageSender(object data)
        {
            int index = (int)data;
            Type type = messageSenderTypes[index];
            manager.AddNewMessageSender(type);

            Debug.Log(string.Format("{0} added.", type.Name));
        }

        public void AddAllMessageHandler()
        {
            for (int i = 0; i < messageHandlerTypes.Length; i++)
            {
                AddMessageHandler(i);
            }
        }
    }
}