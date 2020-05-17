using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class RandomStringGenerator {
    private const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public static string Generate(int length=40) {
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; ++i) {
            result.Append(alphabet[Random.Range(0, alphabet.Length)]);
        }
        return result.ToString();
    }
}
