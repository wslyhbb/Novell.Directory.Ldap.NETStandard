/******************************************************************************
* The MIT License
* Copyright (c) 2003 Novell Inc.  www.novell.com
*
* Permission is hereby granted, free of charge, to any person obtaining  a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including  without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to  permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Novell.Directory.Ldap
{
    /// <summary>
    ///     The. <code>MessageVector</code> class implements additional semantics
    ///     to Vector needed for handling messages.
    /// </summary>
    // TODO - This is locking on this because the same lock is used outside the type - this needs to be improved before removing lock(this)
#pragma warning disable CA2002 // Do not lock on objects with weak identity
    internal class MessageVector
    {
        private readonly List<object> _list;

        internal MessageVector(int cap)
        {
            _list = new List<object>(cap);
        }

        /// <summary>
        ///     Returns an array containing all of the elements in this MessageVector.
        ///     The elements returned are in the same order in the array as in the
        ///     Vector.  The contents of the vector are cleared.
        /// </summary>
        /// <returns>
        ///     the array containing all of the elements.
        /// </returns>
        internal object[] RemoveAll()
        {
            lock (this)
            {
                var results = ToArray();
                _list.Clear();
                return results;
            }
        }

        /// <summary>
        ///     Finds the Message object with the given MsgID, and returns the Message
        ///     object. It finds the object and returns it in an atomic operation.
        /// </summary>
        /// <param name="msgId">
        ///     The msgId of the Message object to return.
        /// </param>
        /// <returns>
        ///     The Message object corresponding to this MsgId.
        ///     @throws NoSuchFieldException when no object with the corresponding
        ///     value for the MsgId field can be found.
        /// </returns>
        internal Message FindMessageById(int msgId)
        {
            lock (this)
            {
                var message = _list.OfType<Message>().SingleOrDefault(m => m.MessageId == msgId);
                if (message == null)
                {
                    throw new FieldAccessException();
                }

                return message;
            }
        }

        /// <summary>
        ///     Adds an object to the end of the Vector.
        /// </summary>
        public void Add(object message)
        {
            lock (this)
            {
                _list.Add(message);
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the Vector.
        /// </summary>
        public void Remove(object message)
        {
            lock (this)
            {
                _list.Remove(message);
            }
        }

        /// <summary>
        ///     Gets the number of elements actually contained in the Vector.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    return _list.Count;
                }
            }
        }

        /// <summary>
        ///     Gets the Message at the specified index.
        /// </summary>
        public object this[int index]
        {
            get
            {
                lock (this)
                {
                    return _list[index];
                }
            }
        }

        /// <summary>
        ///     Removes the element at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            lock (this)
            {
                _list.RemoveAt(index);
            }
        }

        /// <summary>
        ///     Copies the elements to a new array.
        /// </summary>
        public object[] ToArray()
        {
            lock (this)
            {
                return _list.ToArray();
            }
        }

        /// <summary>
        ///     Removes all elements.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                _list.Clear();
            }
        }
    }
#pragma warning restore CA2002 // Do not lock on objects with weak identity
}
