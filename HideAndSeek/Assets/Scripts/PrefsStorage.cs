public interface PrefsStorage {
    T Deserealize<T>(string key) where T : class;
    void Serialize<T>(string key, T obj);
}