using System;
using System.Web;
using System.Collections;

using Cuyahoga.Core;

namespace Cuyahoga.Web.Cache
{
	/// <summary>
	/// The NodeCache stores a list of root nodes and an indexed list of all nodes.
	/// </summary>
	public class NodeCache
	{
		private SortedList _rootNodes;
		private Hashtable _nodeIndex;

		/// <summary>
		/// Property RootNodes (Hashtable)
		/// </summary>
		public SortedList RootNodes
		{
			get { return this._rootNodes; }
			set { this._rootNodes = value; }
		}

		/// <summary>
		/// Property NodeIndex (Hashtable)
		/// </summary>
		public Hashtable NodeIndex
		{
			get { return this._nodeIndex; }
			set { this._nodeIndex = value; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public NodeCache()
		{
			this._rootNodes = new SortedList();
			this._nodeIndex = new Hashtable();
		}

	}
}