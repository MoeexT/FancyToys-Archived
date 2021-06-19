using System;
using System.Collections.Generic;
using System.Text;

namespace FancyUtil.Collection
{
    class FrozenHeap<T>
    {
        private T[] data;
        private int size;
        private int pointer;
        private Comparer<T> comparer;

        public int Size
        {
            get => size;
            set => size = value;
        }

        public FrozenHeap(int capacity)
        {
            this.size = 0;
            this.pointer = 0;
            this.data = new T[capacity + 2];
            this.comparer = null;
        }

        public FrozenHeap(int capacity, Comparer<T> comparer)
        {
            this.size = 0;
            this.pointer = 0;
            this.comparer = comparer;
            this.data = new T[capacity + 2];
        }

        /// <summary>
        /// 入堆
        /// </summary>
        /// <param name="t"></param>
        public void Add(T t)
        {
            size++;
            data[++pointer] = t;
            int idx = pointer;

            while (idx >= 2)
            {
                if (comparer.Compare(data[idx >> 1], data[idx]) > 0)
                {
                    Swap(idx >> 1, idx);
                }
                idx >>= 1;
            }
            if (pointer == data.Length - 1)
            {
                _ = Delete();
            }
        }

        /// <summary>
        /// 出堆
        /// </summary>
        /// <returns></returns>
        public T Delete()
        {
            T res = data[1];
            data[1] = data[pointer];
            data[pointer--] = default;
            int idx = 1;

            while (idx < pointer)
            {
                int left = idx << 1, right = (idx << 1) + 1;
                if (right <= pointer)
                {
                    if (comparer.Compare(data[left], data[right]) >= 0)
                    {
                        Swap(idx, right);
                        idx = right;
                    }
                    else
                    {
                        Swap(idx, left);
                        idx = left;
                    }
                }
                else if (left == pointer)
                {
                    Swap(idx, left);
                    break;
                }
                else
                {
                    break;
                }
            }
            size--;

            return res;
        }

        public bool IsFull()
        {
            return pointer == size - 2;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public override string ToString()
        {
            return data.ToString();
        }



        /// <summary>
        /// 交换堆中两元素
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        private void Swap(int p, int q)
        {
            if (p > pointer || q > pointer || p < 1 || q < 1)
            {
                throw new IndexOutOfRangeException();
            }
            T tmp = data[p];
            data[p] = data[q];
            data[q] = tmp;
        }
    }
}

