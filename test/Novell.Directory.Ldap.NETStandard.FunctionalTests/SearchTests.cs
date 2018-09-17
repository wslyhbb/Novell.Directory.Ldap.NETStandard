using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap.Controls;
using Novell.Directory.Ldap.NETStandard.FunctionalTests.Helpers;
using Xunit;

namespace Novell.Directory.Ldap.NETStandard.FunctionalTests
{
    public class SearchTests
    {
        [Fact]
        public void Can_Search_ByCn()
        {
            const int noOfEntries = 10;
            var ldapEntries = Enumerable.Range(1, noOfEntries).Select(x => LdapOps.AddEntry()).ToList();
            var ldapEntry = ldapEntries[new Random().Next() % noOfEntries];
            TestHelper.WithAuthenticatedLdapConnection(
                ldapConnection =>
                {
                    var lsc = ldapConnection.Search(TestsConfig.LdapServer.BaseDn, LdapConnection.ScopeSub, "cn=" + ldapEntry.GetAttribute("cn").StringValue, null, false);
                    var entries = lsc.ToList();

                    Assert.Single(entries);
                    ldapEntry.AssertSameAs(entries[0]);
                });
        }

        [Fact]
        public void Can_Search_Using_Vlv()
        {
            const int pages = 10;
            const int pageSize = 10;
            var cnPrefix = "something";
            var ldapEntries = Enumerable.Range(1, pages  * pageSize).Select(x => LdapOps.AddEntry(cnPrefix)).ToList();

            var searchConstraints = new LdapSearchConstraints
            {
                BatchSize = 1,
                MaxResults = 10000
            };

            TestHelper.WithAuthenticatedLdapConnection(
                ldapConnection =>
                {
                    var entries = new List<LdapEntry>();
                    var sortControl = new LdapSortControl(new LdapSortKey("cn"), true);
                    for (var i = 0; i < pages; i++)
                    {
                        searchConstraints.SetControls(new LdapControl[] { BuildLdapVirtualListControl(i+1, pageSize), sortControl });
                        var lsc = ldapConnection.Search(TestsConfig.LdapServer.BaseDn, LdapConnection.ScopeSub, "cn=" + cnPrefix + "*", null, false, searchConstraints);
                        entries.AddRange(lsc.ToList());
                    }

                    Assert.Equal(ldapEntries.Count, entries.Count);
                });
        }

        private static LdapVirtualListControl BuildLdapVirtualListControl(int page, int pageSize)
        {
            var startIndex = (page - 1) * pageSize;
            startIndex++;
            var beforeCount = 0;
            var afterCount = pageSize - 1;
            var contentCount = 0;

            return new LdapVirtualListControl(startIndex, beforeCount, afterCount, contentCount);            
        }
    }
}
