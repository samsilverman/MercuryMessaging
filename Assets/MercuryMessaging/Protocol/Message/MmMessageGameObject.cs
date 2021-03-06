﻿// Copyright (c) 2017, Columbia University 
// All rights reserved. 
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of Columbia University nor the names of its 
//    contributors may be used to endorse or promote products derived from 
//    this software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 
// 
// =============================================================
// Authors: 
// Carmine Elvezio, Mengu Sukan, Samuel Silverman, Steven Feiner
// =============================================================
// 
// 
using UnityEngine;
using System.Linq;
#if PHOTON_AVAILABLE
using Photon.Pun;
#endif

namespace MercuryMessaging
{
    /// <summary>
    /// MmMessage with MmMessageGameObject payload
    /// </summary>
	public class MmMessageGameObject : MmMessage
	{
        /// <summary>
        /// MmMessageGameObject GameObject payload
        /// </summary>
		public GameObject Value;

        /// <summary>
        /// Creates a basic MmMessageGameObject
        /// </summary>
		public MmMessageGameObject()
		{}

        /// <summary>
        /// Creates a basic MmMessageGameObject, with a control block
        /// </summary>
        /// <param name="metadataBlock">Object defining the routing of messages</param>
		public MmMessageGameObject(MmMetadataBlock metadataBlock = null)
			: base (metadataBlock, MmMessageType.MmGameObject)
		{
		}

        /// <summary>
        /// Create an MmMessage, with control block, MmMethod, and an GameObject
        /// </summary>
        /// <param name="transform">MmTransform payload</param>
        /// <param name="mmMethod">Identifier of target MmMethod</param>
        /// <param name="metadataBlock">Object defining the routing of messages</param>
        public MmMessageGameObject(GameObject gameObject,
            MmMethod mmMethod = default(MmMethod),
            MmMetadataBlock metadataBlock = null)
            : base(mmMethod, MmMessageType.MmGameObject, metadataBlock)
        {
            Value = gameObject;
        }

        /// <summary>
        /// Duplicate an MmMessage
        /// </summary>
        /// <param name="message">Item to duplicate</param>
        public MmMessageGameObject(MmMessageGameObject message) : base(message)
		{}

        /// <summary>
        /// Message copy method
        /// </summary>
        /// <returns>Duplicate of MmMessage</returns>
        public override MmMessage Copy()
        {
            MmMessageGameObject newMessage = new MmMessageGameObject(this);
            newMessage.Value = Value;

            return newMessage;
        }

        /// <summary>
        /// Deserialize the MmMessageGameObject
        /// </summary>
        /// <param name="data">Object array representation of a MmMessageGameObject</param>
        /// <returns>The index of the next element to be read from data</returns>
        public override int Deserialize(object[] data)
        {
            int index = base.Deserialize(data);
            #if PHOTON_AVAILABLE
            bool networkedGameObject = (bool) data[index++];
            if (networkedGameObject)
            {
                int photonViewId = (int) data[index++];
                PhotonView photonView = PhotonView.Find(photonViewId);
                Value = null;
                if (photonView != null)
                {
                    Value = photonView.gameObject;
                }
            }
            else 
            {
                int instanceID = (int) data[index++];
                Value = GameObject.Find(instanceID.ToString());
            }
            #else
            int instanceID = (int) data[index++];
            Value = GameObject.Find(instanceID.ToString());
            #endif
            return index;
        }

        /// <summary>
        /// Serialize the MmMessageGameObject
        /// </summary>
        /// <returns>Object array representation of a MmMessageGameObject</returns>
        public override object[] Serialize()
        {
            object[] baseSerialized = base.Serialize();
            object[] thisSerialized; 

            #if PHOTON_AVAILABLE
            if (Value.GetComponent<PhotonView>() != null)
            {
                PhotonView photonView = Value.GetComponent<PhotonView>();
                thisSerialized = new object[] { true, photonView.ViewID };
            }
            else 
            {
                thisSerialized = new object[] { false, Value.GetInstanceID() };
            }
            #else
            thisSerialized = new object[] { Value.GetInstanceID() };
            #endif

            
            object[] combinedSerialized = baseSerialized.Concat(thisSerialized).ToArray();
            return combinedSerialized;
        }
	}
}