using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour
{
    // Class used on empty piece to trigger an event and perform the piece movement
    #region Exposed
    
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        _transform = this.transform;
        _startPosition = _transform.position;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods

    // event called when a gameObject is dropped on it.
    public void Drop(BaseEventData eventData) {
        GameObject draggedSticker = ((PointerEventData)eventData).pointerDrag;
        int draggedPieceIndex = draggedSticker.transform.GetSiblingIndex();
        int emptyPieceIndex = _transform.GetSiblingIndex();

        
        // switch transform index in hierarchy if they are neighbours.

        if (IsNeighbour(draggedPieceIndex, emptyPieceIndex)) {
            _transform.SetSiblingIndex(draggedPieceIndex);
            draggedSticker.transform.SetSiblingIndex(emptyPieceIndex);

            // call the event on Game Manager
            GameManager.Instance._onPieceMoved?.Invoke(draggedPieceIndex, emptyPieceIndex);
        }
    }

    private bool IsNeighbour(int pieceIndex1, int pieceIndex2) {
        // calculate piece coordinates on 3x3 grid.
        int piece1X = pieceIndex1%3;
        int piece1Y = pieceIndex1/3;
        int piece2X = pieceIndex2%3;
        int piece2Y = pieceIndex2/3;
        
        return new Vector2(piece2X-piece1X,piece2Y-piece1Y).magnitude == 1;
    }
    #endregion

    #region Private & Protected
    private Transform _transform;
    private Vector3 _startPosition;
    #endregion
}
