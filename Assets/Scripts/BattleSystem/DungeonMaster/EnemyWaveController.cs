using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveController : MonoBehaviour
{
	[SerializeField] int minSpawnDistance;
	private List<Vector2Int> GetEnemySpawnPos()
	{
		int h = BattleManager.Instance.hexgrid.Height;
		int w = BattleManager.Instance.hexgrid.Width;
		float[,] rawEnemySpawnMap = NoiseSystem.GenerateNoiseMap(w, h, 0.5f);
		List<Vector2Int> enemySpawnMap = NoiseSystem.BlobDetection(rawEnemySpawnMap, 0.5f, 10);
		return enemySpawnMap;
	}
	IEnumerator EnemyWave(int enemyCount, float waveDuration)
	{
		int cnt = enemyCount;
		List<Vector2Int> enemyMap = GetEnemySpawnPos();
		while (cnt>0)
		{
			//spawn enemy
			while (true)
			{
				int randIndex = Random.Range(0, enemyMap.Count);
				Vector2Int pos = enemyMap[randIndex];
				Vector2Int randVector = new Vector2Int((int)Random.Range(-10f, 10f), (int)Random.Range(-10f, 10f));
				pos+=randVector;
				if (!BattleManager.Instance.hexgrid.IsValidCell(pos)) continue;
				EnemyManager.Instance.InstanciateEnemy(pos);
				break;
			}
			cnt--;
			yield return waveDuration/enemyCount;
		}
	}
}