using System;
using System.Collections;

namespace LibraProgramming.Windows.Collections
{
/*
    public partial class SourceCollectionView
    {
        /// <summary>
        /// 
        /// </summary>
        private class FilteredEnumerator : PlainEnumerator
        {
            private Predicate<object> predicate;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="predicate"></param>
            public FilteredEnumerator(IEnumerable source, Predicate<object> predicate)
                : base(source)
            {
                this.predicate = predicate;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override bool MoveNext()
            {
                while (base.MoveNext())
                {
                    if (predicate(Current))
                    {
                        return true;
                    }
                }

                return false;
            }

            protected override void DisposeOverride()
            {
                base.DisposeOverride();
                predicate = null;
            }
        }
    }
*/
}