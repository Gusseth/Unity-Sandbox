using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public static class Algorithms
{
    public const int InsertionSortThreshold = 16;

    /// <summary>
    /// Comparison function for RaycastHit-related sorts based on distance.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><list type="bullet">
    /// <item>
    /// <term>-1</term> <description>If X is closer than Y</description>
    /// </item>
    /// <item>
    /// <term>0</term> <description>If both are the same distance away</description>
    /// </item>
    /// <item>
    /// <term>1</term> <description>If X is further than Y</description>
    /// </item>
    /// </list></returns>
    public static int RaycastHitDistanceComparer(RaycastHit x, RaycastHit y)
    {
        switch (x.distance - y.distance)
        {
            case < 0: return -1;
            case > 0: return 1;
        }
        return 0;
    }

    /// <summary>
    /// Sorts an array in-place without heap allocation.
    /// </summary>
    /// <remarks>This method automatically chooses what algorithm the sort will use.</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array to sort</param>
    /// <param name="startIndex">The start of the range you want to sort</param>
    /// <param name="length">The number of elements including array[startIndex] to include in the sort</param>
    /// <param name="compare">The comparison function that will be used</param>
    public static void SortArray<T>(ref T[] array, int startIndex, int length, Comparison<T> compare)
    {
        switch (length)
        {
            case < 0:
            case 1:
                return;
            case <= InsertionSortThreshold:
                InsertionSort(ref array, startIndex, length, compare);
                return;
            case > InsertionSortThreshold:
                QuickSort(ref array, startIndex, startIndex + (length - 1), compare);
                return;
        }
    }

    public static void Swap<T>(ref T[] array, in int x, in int y)
    {
        (array[x], array[y]) = (array[y], array[x]);
    }

    /// <summary>
    /// Sorts an array in-place without heap allocation with Insertion Sort.
    /// </summary>
    /// <remarks>I recommend that the sorting range must be at most (inclusive) 16 elements long because this is O(n^2).</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array to sort</param>
    /// <param name="startIndex">The start of the range you want to sort</param>
    /// <param name="length">The number of elements including array[startIndex] to include in the sort</param>
    /// <param name="compare">The comparison function that will be used</param>
    public static void InsertionSort<T>(ref T[] array, int startIndex, int length, Comparison<T> compare)
    {
        switch (length)
        {
            case <= 1:
                return;
            case 2:
                if (compare(array[startIndex], array[startIndex + 1]) > 0)
                    Swap(ref array, startIndex, startIndex + 1);
                return;
        }

        for (int i = startIndex; i < length; i++)
        {
            int j = i - 1;
            T x = array[i];
            while (j >= startIndex && compare(array[j], x) > 0)
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = x;
        }
    }

    /// <summary>
    /// Sorts an array in-place without heap allocation with QuickSort.
    /// </summary>
    /// <remarks>I recommend you use this for sorts longer than 16 elements.</remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array to sort</param>
    /// <param name="startIndex">The start of the range you want to sort</param>
    /// <param name="high">The end of the range you want to sort (inclusive)</param>
    /// <param name="compare">The comparison function that will be used</param>
    public static void QuickSort<T>(ref T[] array, int startIndex, int high, Comparison<T> compare)
    {
        if (startIndex >= 0 && high >= 0 && startIndex < high)
        {
            if (startIndex - high == 1)
            {
                if (compare(array[startIndex], array[high]) > 0)
                    Swap(ref array, startIndex, high);
                return;
            }

            int partition = QuicksortPartition(ref array, startIndex, high, compare);
            QuickSort(ref array, startIndex, partition, compare);
            QuickSort(ref array, partition + 1, high, compare);
        }
    }

    /// <summary>
    /// Middle-of-the-array based quicksort pivot selection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="high"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    private static int QuicksortPartition<T>(ref T[] array, int startIndex, int high, Comparison<T> compare)
    {
        T pivot = array[(high - startIndex) / 2 + startIndex];
        int i = startIndex - 1;
        int j = high + 1;

        while (true)
        {
            do
            {
                i++;
            }
            while (compare(array[i], pivot) < 0);

            do
            {
                j--;
            }
            while (compare(array[j], pivot) > 0);
            if (i >= j) return j;
            Swap(ref array, i, j);
        }
    }
}
