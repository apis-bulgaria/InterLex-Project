using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class ArticleData
    {
        private string topMostParent;

        public ArticleData(string eid, string num)
        {
            this.Eid = eid;
            this.Num = num;
        }

        public string Eid { get; }

        public string Num { get; }

        public string TopMostParent
        {
            get
            {
                if (this.topMostParent == null)
                {
                    string[] splitEid = Eid?.Split(new string[] { "__" }, StringSplitOptions.None);
                    string chapterPart = splitEid.Where(se => se.Contains("chap")).FirstOrDefault();
                    this.topMostParent = splitEid[0] + chapterPart;
                }

                return this.topMostParent;
            }
        }

        public override int GetHashCode()
        {
            // We don't care about the integer overflow
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, this.Eid) ? this.Eid.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!object.ReferenceEquals(null, this.Num) ? this.Num.GetHashCode() : 0);
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var articleData = obj as ArticleData;

            return !object.ReferenceEquals(null, articleData)
                && string.Equals(this.Eid, articleData.Eid)
                && this.Num == articleData.Num;
        }
    }
}
