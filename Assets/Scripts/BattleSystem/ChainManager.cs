using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ChainPattern
{
    public string chainName;
    public List<Suit> pattern;
    public Action onComplete;
    
    public ChainPattern(string chainName,List<Suit> pattern, Action onComplete)
    {
        this.chainName = chainName;
        this.pattern = pattern;
        this.onComplete = onComplete;
    }
}

public class ChainManager : Singleton<ChainManager>
{
    private Queue<Suit> recordedChain = new Queue<Suit>();
    [SerializeField] private List<ChainPattern> chainLibrary = new List<ChainPattern>();
    
    [SerializeField] private int maxChainLength = 5; // Maximum length to store

    public Action<Suit> OnEnqueueSuitChain;
    private void Awake()
    {
        InitializeChainLibrary();
    }

    private void InitializeChainLibrary()
    {
        // Example chain patterns
        AddChainPattern("UltimateFireball",
            new List<Suit> { Suit.Time, Suit.Eclipse, Suit.Spirit },
            () => Debug.Log("Fireball Chain Activated!")
        );

        AddChainPattern("UltimateIceStorm",
            new List<Suit> { Suit.Spirit, Suit.Spirit, Suit.Time },
            () => Debug.Log("Ice Storm Chain Activated!")
        );
    }

    public void AddChainPattern(string chainName,List<Suit> pattern, Action callback)
    {
        chainLibrary.Add(new ChainPattern(chainName,pattern, callback));
    }

    public void RecordSuit(Suit suit)
    {
        // Add new suit to chain
        recordedChain.Enqueue(suit);

        // Maintain maximum length
        if (recordedChain.Count > maxChainLength)
        {
            recordedChain.Dequeue();
        }

        OnEnqueueSuitChain?.Invoke(suit);
        // Check for matching patterns
        CheckForMatches();
    }

    public Suit GetLastSuit()
    {
        return recordedChain.Count > 0 ? recordedChain.Last() : Suit.Time; // Default to Time if empty
    }

    public List<Suit> GetChainInRange(int count)
    {
        return recordedChain.Reverse().Take(count).Reverse().ToList();
    }

    public List<Suit> GetCurrentChain()
    {
        return recordedChain.ToList();
    }

    private void CheckForMatches()
    {
        List<Suit> currentChain = GetCurrentChain();

        foreach (var pattern in chainLibrary)
        {
            if (IsPatternMatch(currentChain, pattern.pattern))
            {
                pattern.onComplete?.Invoke();
                ClearChain();
                break; // Only trigger the first matching pattern
            }
        }
    }

    private bool IsPatternMatch(List<Suit> currentChain, List<Suit> pattern)
    {
        if (currentChain.Count < pattern.Count) return false;

        // Check if the end of current chain matches the pattern
        return !pattern.Where((suit, i) => 
            suit != currentChain[currentChain.Count - pattern.Count + i]).Any();
    }

    public bool ValidateChain(List<Suit> checkPattern)
    {
        List<Suit> currentChain = GetCurrentChain();
        return IsPatternMatch(currentChain, checkPattern);
    }

    public void ClearChain()
    {
        recordedChain.Clear();
    }

    // Debug method to print current chain
    public void DebugPrintChain()
    {
        string chainString = string.Join(" -> ", GetCurrentChain());
        Debug.Log($"Current Chain: {chainString}");
    }
}