using Firebase.Firestore;

[FirestoreData]
public struct Message
{
    [FirestoreProperty]
    public string Drink1 { get; set; }

    [FirestoreProperty]
    public string Drink2 { get; set; }

    [FirestoreProperty]
    public string Drink3 { get; set; }

    [FirestoreProperty]
    public int DrinkDecoration { get; set; }

    [FirestoreProperty]
    public string DrinkName { get; set; }

    [FirestoreProperty]
    public string OrderNumber { get; set; }

    [FirestoreProperty]
    public string audio { get; set; }

    [FirestoreProperty]
    public string image { get; set; }

    [FirestoreProperty]
    public string text { get; set; }

}
