using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessageObject : MonoBehaviour
{
	public float speed = 300f;

	public void Initialize(string message, Color color, Vector2 vec)
	{
        gameObject.SetActive(true);
        Debug.Log("Start message initialize");

        Text msg = transform.GetComponent<Text>();
		msg.text = message;
        msg.color = color;
        msg.gameObject.SetActive(true);

        var baseY = vec.y;

        transform.position = vec;
        transform.DOKill();
        DOTween.Sequence()
                .OnStart(() =>
                {
                    transform.localScale = Vector3.zero;
                    transform.GetComponent<CanvasGroup>().alpha = 1;
                })
                .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce))
                .Join(transform.DOLocalMoveY(baseY + 10f, 1f).SetEase(Ease.OutCirc))
                .Join(transform.DOLocalMoveZ(0, 3f).SetEase(Ease.OutCirc))
                .Join(transform.GetComponent<CanvasGroup>().DOFade(0f, 3f).SetEase(Ease.OutCirc))
                .OnComplete(() =>
				{
					gameObject.SetActive(false);
				}).Play();
	}
}

