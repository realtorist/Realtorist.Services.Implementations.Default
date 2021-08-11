using Realtorist.Models.Collections;
using Realtorist.Services.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtorist.Services.Implementations.Default.Cache
{
    /// <summary>
    /// Implements cache using in-memory with LFU strategy
    /// </summary>
    /// <typeparam name="T">Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    public class InMemoryLFUCache<T, V> : LFUCache<T, V>, ICache<T, V>
    {
        /// <summary>
        /// Creates new intance of <see cref="InMemoryLFUCache{T, V}"/>
        /// </summary>
        /// <param name="capacity">Max Capacity</param>
        public InMemoryLFUCache(int capacity) : base(capacity)
        {
        }
    }
}
