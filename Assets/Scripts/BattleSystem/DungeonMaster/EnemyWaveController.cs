using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyWaveController : MonoBehaviour
{
	[SerializeField] int minSpawnDistance;

	private void Start()
	{
	}

	private Vector2Int GetEnemySpawnPos()
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		int randomX;
		int randomY;
		do
		{
			randomX = Random.Range(0, w);
			randomY = Random.Range(0, h);
		} while (!BattleManager.Instance.hexgrid.IsValidCell(new Vector2Int(randomX, randomY)));
		return new Vector2Int(randomX, randomY);
	}
	public IEnumerator EnemyWave(int enemyCount, float waveDuration)
	{
		int cnt = enemyCount;
		while (cnt>0)
		{
			Vector2Int pos = GetEnemySpawnPos();
			EnemyManager.Instance.InstanciateEnemy(pos);
			cnt--;
			yield return new WaitForSeconds(waveDuration/enemyCount);
		}
	}
}