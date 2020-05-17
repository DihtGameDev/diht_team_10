using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringsList { // simple strings list, because collection not working for serialization
    public string[] buffer;
    public int capacity = 0;
    public int size = 0;

    private int _additionalCount = 15;

    public StringsList(int capacity=5) {
        this.capacity = capacity;
        buffer = new string[capacity];
    }
    
    public StringsList(string[] input) {
        capacity = input.Length + _additionalCount;
        size = input.Length;

        buffer = new string[capacity];

        for (int i = 0; i < input.Length; ++i) {
            buffer[i] = input[i];
        }
    }

    public bool Contains(string value) {
        for (int i = 0; i < size; ++i) {
            Debugger.Log("Constains: " + buffer[i]);
            if (buffer[i] == value) {
                return true;
            }
        }

        return false;
    }

    public void Append(string value) {
        ReserveIfNeeds();
        buffer[size] = value;
        ++size;
    }

    public void Set(int index, string value) {
        CheckAccessToIndex(index);
        buffer[index] = value;
    }

    public string Get(int index) {
        CheckAccessToIndex(index);
        return buffer[index];
    }

    private void ReserveIfNeeds() {
        if (size - 1 >= capacity) {
            capacity += _additionalCount;
            string[] newBuffer = new string[capacity];
            for (int i = 0; i < buffer.Length; ++i) {
                newBuffer[i] = buffer[i];
            }
            buffer = newBuffer;
        }
    }

    private void CheckAccessToIndex(int index) {
        if (index + 1 >= size) {
            throw new System.IndexOutOfRangeException();
        }
    }
}
