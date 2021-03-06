﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.TreeView;

namespace dnSpy.AsmEditor.Commands {
	sealed class DeletedTypeUpdater {
		public IEnumerable<DocumentTreeNodeData> OriginalNodes {
			get { yield return ownerNode; }
		}

		readonly TreeNodeData parentNode;
		readonly TypeNode ownerNode;
		readonly ModuleDef ownerModule;
		readonly TypeDef ownerType;
		readonly TypeDef type;
		int typeIndex;

		public DeletedTypeUpdater(ModuleDocumentNode modNode, TypeDef originalType) {
			this.ownerNode = modNode.Context.DocumentTreeView.FindNode(originalType);
			if (ownerNode == null)
				throw new InvalidOperationException();
			this.parentNode = ownerNode.TreeNode.Parent.Data;
			this.ownerModule = originalType.Module;
			this.ownerType = originalType.DeclaringType;
			this.type = originalType;
		}

		public void Add() {
			if (!parentNode.TreeNode.Children.Remove(ownerNode.TreeNode))
				throw new InvalidOperationException();
			if (ownerType != null) {
				this.typeIndex = ownerType.NestedTypes.IndexOf(type);
				ownerType.NestedTypes.RemoveAt(typeIndex);
			}
			else {
				this.typeIndex = ownerModule.Types.IndexOf(type);
				ownerModule.Types.RemoveAt(typeIndex);
			}
		}

		public void Remove() {
			if (ownerType != null)
				ownerType.NestedTypes.Insert(typeIndex, type);
			else
				ownerModule.Types.Insert(typeIndex, type);
			parentNode.TreeNode.AddChild(ownerNode.TreeNode);
		}
	}
}
