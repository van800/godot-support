using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.DeclaredElements;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Impl.Search.SearchDomain;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Godot.Tscn.Psi.Search
{
    /*
    [PsiSharedComponent]
    public class TscnUsageSearchFactory : IDomainSpecificSearcherFactory
    {
        private readonly SearchDomainFactory mySearchDomainFactory;

        public TscnUsageSearchFactory(SearchDomainFactory searchDomainFactory)
        {
            mySearchDomainFactory = searchDomainFactory;
        }

        public bool IsCompatibleWithLanguage(PsiLanguageType languageType)
        {
            return languageType.Is<TscnLanguage>();
        }

        public IDomainSpecificSearcher CreateConstructorSpecialReferenceSearcher(ICollection<IConstructor> constructors)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateMethodsReferencedByDelegateSearcher(IDelegate @delegate)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateReferenceSearcher(IDeclaredElementsSet elements, bool findCandidates)
        {
            if (elements.Any(element => !(element is ITscnDeclaredElement)))
                return null;
            return new TscnReferenceSearcher(elements, findCandidates);
        }

        public IDomainSpecificSearcher CreateLateBoundReferenceSearcher(IDeclaredElementsSet elements)
        {
            return null;
        }

        // Scenes and resources typically do not reference code elements in literals
        // and comments are discarded when modifying the file by the Godot Editor.
        // It is unlikely any useful text occurence would appear here.
        public IDomainSpecificSearcher CreateTextOccurrenceSearcher(IDeclaredElementsSet elements)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateTextOccurrenceSearcher(string subject)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateAnonymousTypeSearcher(IList<AnonymousTypeDescriptor> typeDescription, bool caseSensitive)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateConstantExpressionSearcher(ConstantValue constantValue, bool onlyLiteralExpression)
        {
            return null;
        }

        public IEnumerable<string> GetAllPossibleWordsInFile(IDeclaredElement element)
        {
            yield return element.ShortName;
        }

        public IEnumerable<RelatedDeclaredElement> GetRelatedDeclaredElements(IDeclaredElement element)
        {
            return EmptyList<RelatedDeclaredElement>.InstanceList;
        }

        public IEnumerable<FindResult> GetRelatedFindResults(IDeclaredElement element)
        {
            return EmptyList<FindResult>.InstanceList;
        }

        public Tuple<ICollection<IDeclaredElement>, Predicate<IFindResultReference>, bool> GetDerivedFindRequest(IFindResultReference result)
        {
            return null;
        }

        public Tuple<ICollection<IDeclaredElement>, bool> GetNavigateToTargets(IDeclaredElement element)
        {
            return null;
        }

        public ICollection<FindResult> TransformNavigationTargets(ICollection<FindResult> targets)
        {
            return targets;
        }

        public ISearchDomain GetDeclaredElementSearchDomain(IDeclaredElement declaredElement)
        {
            if (!(declaredElement is ITscnDeclaredElement))
                return EmptySearchDomain.Instance;

            return mySearchDomainFactory.CreateSearchDomain(declaredElement.GetSourceFiles());
        }
    }
    */
}