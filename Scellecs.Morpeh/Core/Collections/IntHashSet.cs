using System;

namespace Scellecs.Morpeh.Collections {
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class IntHashSet : IDisposable {
        public int length;
        public int capacity;
        public int capacityMinusOne;
        public int lastIndex;
        public int freeIndex;

        public IntPinnedArray buckets;
        public IntPinnedArray slots;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntHashSet() : this(0) {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntHashSet(int capacity) {
            this.lastIndex = 0;
            this.length    = 0;
            this.freeIndex = -1;

            this.capacityMinusOne = HashHelpers.GetCapacity(capacity);
            this.capacity         = this.capacityMinusOne + 1;
            this.buckets          = new IntPinnedArray(this.capacity);
            this.slots            = new IntPinnedArray(this.capacity * 2);
        }

        public void Dispose() {
            this.lastIndex = 0;
            this.length = 0;
            this.freeIndex = -1;
            this.capacityMinusOne = 0;
            this.capacity = 0;
            this.buckets.Dispose();
            this.slots.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() {
            Enumerator e;
            e.set     = this;
            e.index   = 0;
            e.current = default;
            return e;
        }

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        public unsafe struct Enumerator {
            public IntHashSet set;

            public int index;
            public int current;

            public bool MoveNext() {
                var slotsPtr = this.set.slots.ptr;
                for (var len = this.set.lastIndex; this.index < len; this.index += 2) {
                    var v = slotsPtr[this.index] - 1;
                    if (v < 0) {
                        continue;
                    }

                    this.current = v;
                    this.index += 2;
                    return true;
                }

                this.index = this.set.lastIndex + 1;
                this.current = default;
                return false;
            }

            public int Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.current;
            }
        }
    }
}