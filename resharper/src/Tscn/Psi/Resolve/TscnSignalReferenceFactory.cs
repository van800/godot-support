using JetBrains.Annotations;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Parsing.TokenNodeTypes;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Resolve
{
    public class TscnSignalReferenceFactory : IReferenceFactory
    {
        public bool HasReference(ITreeNode element, IReferenceNameContainer names)
        {
            if (element is IVariantLiteral literal && literal.NodeType == TscnTokenNodeTypes.STRING_LITERAL)
            {
                // TODO: An extension method or a method in IVariantLiteral to get string value
                var textWithQuotes = literal.GetText();
                var text = textWithQuotes.Substring(1, textWithQuotes.Length - 2);
                return names.Contains(text);
            }

            return false;
        }

        public ReferenceCollection GetReferences(ITreeNode element, ReferenceCollection oldReferences)
        {
            // TODO: Is this a good idea? FSharp does not seem to do this, but asmdef does.
            if (ResolveUtil.CheckThatAllReferencesBelongToElement<TscnSignalReference>(oldReferences, element))
                return oldReferences;

            if (!(element is IVariantLiteral literal))
                return ReferenceCollection.Empty;
            if (!IsLocalSignalMethodLiteral(literal))
                return ReferenceCollection.Empty;

            if (!(literal.GetContainingFile() is ITscnFile tscnFile))
                return ReferenceCollection.Empty;

            // TODO: Verify this is not too performance intensive, there might be an API for this
            // TODO: Is using LINQ ok here? I am guessing not.
            foreach (var sceneElement in tscnFile.SceneContents)
            {
                // This implementation only supports one simple case - a method reference to the root node
                // if the root node uses a C# script that is defined in an external resource
                // TODO: Support non-root nodes
                // TODO: Support non-external-resource scripts
                // TODO: Split this into reusable methods. Perhaps define them on the Tree nodes.
                if (sceneElement.Element is INode node)
                {
                    bool parentFound = false;
                    foreach (var value in node.HeadingValues)
                    {
                        if (value.Identifier.GetText() == "parent")
                        {
                            parentFound = true;
                            break;
                        }
                    }

                    // We are only looking for a node that has no parent - that is the root node
                    if (parentFound)
                        continue;

                    foreach (var nodeValue in node.Values)
                    {
                        if (nodeValue.Identifier.GetText() != "script") continue;
                        var externalResourceId = GetExternalResourceId(nodeValue.Value);
                        if (externalResourceId < 0) continue;

                        var externalResource = GetExternalResourceById(tscnFile, externalResourceId);
                        if (externalResource == null) continue;

                        string type = null;
                        IVariantLiteral pathLiteral = null;
                        foreach (var value in externalResource.Values)
                        {
                            switch (value.Identifier.GetText())
                            {
                                case "path":
                                    if (value.Value is IVariantLiteral variantLiteral)
                                        pathLiteral = variantLiteral;
                                    break;
                                case "type":
                                    // TODO: There has to be an utility method for this somewhere, or we should make our own
                                    type = value.Value.GetText();
                                    type = type.Substring(1, type.Length - 2);
                                    break;
                            }
                        }

                        // TODO: For now we only support C# scripts, not references across multiple scene files
                        if (type != "Script" || pathLiteral == null) continue;

                        var reference = new TscnSignalReference(literal, pathLiteral,  literal.Literal)
                        return new ReferenceCollection(reference);
                    }
                }
            }

            return ReferenceCollection.Empty;
        }

        private bool IsLocalSignalMethodLiteral(IVariantLiteral literal)
        {
            var valuePair = KeyValuePairNavigator.GetByValue(literal);
            var connection = ConnectionNavigator.GetByValue(valuePair);

            if (connection == null) return false;
            if (valuePair?.GetText() != "method") return false;

            // TODO: We currently only support references to methods of the main node in a scene.
            foreach (var value in connection.ValuesEnumerable)
            {
                if (value.Identifier.GetText() == "to")
                {
                    // "." denotes the root node of a scene
                    return value.Value.GetText() == "\".\"";
                }
            }

            return true;
        }

        // TODO: Consider moving to a separate utility class
        private int GetExternalResourceId(ITreeNode node)
        {
            if (!(node is IVariantConstructor constructor)) return -1;

            if (constructor.TypeIdentifier.GetText() != "ExtResource" ||
                constructor.Values.Count != 1)
                return -1;

            if (constructor.Values[0].NodeType != TscnTokenNodeTypes.NUMERIC_LITERAL)
                return -1;

            return int.Parse(constructor.Values[0].GetText());
        }

        [CanBeNull]
        private IExtResource GetExternalResourceById(ITscnFile file, int id)
        {
            // TODO: Support both scene and resource files, currently only scene data is checked
            foreach (var element in file.SceneContents)
            {
                if (element.Element is IExtResource externalResource)
                {
                    foreach (var headingValue in externalResource.HeadingValues)
                    {
                        if (headingValue.Identifier.GetText() != "id") continue;
                        var value = headingValue.Value;
                        if (value.NodeType != TscnTokenNodeTypes.NUMERIC_LITERAL) return null;
                        int resourceId = int.Parse(value.GetText());

                        if (resourceId == id)
                            return externalResource;
                    }
                }
            }

            return null;
        }
    }
}