using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

public class EnemyWaveController : MonoBehaviour
{
	[SerializeField] private TMP_Text waveNum;
	[SerializeField] int minSpawnDistance;
	public int waveCnt = 0;
	
	public IEnumerator EnemyWave(int enemyCount, float waveDuration)
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		waveCnt++;
		waveNum.text = waveCnt.ToString();
		int cnt = enemyCount*waveCnt;
		List<Vector2Int> posList = NoiseSystem.GetPositions(enemyCount,w,h);
		foreach (var p in posList)
		{
			EnemyManager.Instance.InstantiateEnemy(p);
			cnt--;
			yield return new WaitForSeconds(waveDuration/enemyCount);
		}
	}
	
	public IEnumerator BossEnemyWave(int enemyCount, float waveDuration)
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		waveCnt++;
		waveNum.text = "BOSS WAVE";
		int cnt = enemyCount;
		List<Vector2Int> posList = NoiseSystem.GetPositions(enemyCount,w,h);
		foreach (var p in posList)
		{
			EnemyManager.Instance.InstantiateEnemy(p);
			cnt--;
			yield return new WaitForSeconds(waveDuration/enemyCount);
		}
		EnemyManager.Instance.InstantiateBoss();
		SoundManager.Instance.ChangeBossBGM();
	}
}