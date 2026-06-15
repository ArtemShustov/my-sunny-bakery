using MySunnyBakery.Items;
using MySunnyBakery.Production;
using UnityEngine;

namespace MySunnyBakery.Vehicles {
	public class Trunk : Machine {
		[SerializeField] private Vector2Int _gridSize = new Vector2Int(3, 3);
		[SerializeField] private Vector2 _cellSize = new Vector2(1f, 1f);
		[SerializeField] private float _stackVerticalOffset = 0.05f;

		private ItemRack[,] _grid;

		private void Awake() {
			_grid = new ItemRack[_gridSize.x, _gridSize.y];
		}

		public override bool CanReceive(GameObject item) {
			if (!item.TryGetComponent<Item>(out var itemComponent)) {
				return false;
			}

			var itemDef = itemComponent.Definition;

			for (var x = 0; x < _gridSize.x; x++) {
				for (var y = 0; y < _gridSize.y; y++) {
					var rack = _grid[x, y];
					if (rack != null && rack.Definition == itemDef && !rack.IsFull) {
						return true;
					}
				}
			}

			for (var x = 0; x <= _gridSize.x - itemDef.Size.x; x++) {
				for (var y = 0; y <= _gridSize.y - itemDef.Size.y; y++) {
					if (CanPlaceAt(x, y, itemDef.Size)) {
						return true;
					}
				}
			}

			return false;
		}
		public override void Receive(GameObject item) {
			if (!item.TryGetComponent<Item>(out var itemComponent)) {
				return;
			}

			var itemDef = itemComponent.Definition;

			for (var x = 0; x < _gridSize.x; x++) {
				for (var y = 0; y < _gridSize.y; y++) {
					var rack = _grid[x, y];
					if (rack != null && rack.Definition == itemDef && !rack.IsFull) {
						rack.Add(item);
						PositionItemInCell(item, x, y, rack.Size, rack.Items.Count - 1);
						return;
					}
				}
			}

			for (var x = 0; x <= _gridSize.x - itemDef.Size.x; x++) {
				for (var y = 0; y <= _gridSize.y - itemDef.Size.y; y++) {
					if (CanPlaceAt(x, y, itemDef.Size)) {
						var newRack = new ItemRack {
							Definition = itemDef,
							Size = itemDef.Size
						};
						PlaceRackAt(x, y, itemDef.Size, newRack);
						newRack.Add(item);
						PositionItemInCell(item, x, y, newRack.Size, newRack.Items.Count - 1);
						return;
					}
				}
			}
		}

		private bool CanPlaceAt(int originX, int originY, Vector2Int size) {
			for (var dx = 0; dx < size.x; dx++) {
				for (var dy = 0; dy < size.y; dy++) {
					if (_grid[originX + dx, originY + dy] != null) {
						return false;
					}
				}
			}
			return true;
		}

		private void PlaceRackAt(int originX, int originY, Vector2Int size, ItemRack rack) {
			for (var dx = 0; dx < size.x; dx++) {
				for (var dy = 0; dy < size.y; dy++) {
					_grid[originX + dx, originY + dy] = rack;
				}
			}
		}
		private void ClearRackCells(ItemRack rack) {
			for (var x = 0; x < _gridSize.x; x++) {
				for (var y = 0; y < _gridSize.y; y++) {
					if (_grid[x, y] == rack) {
						_grid[x, y] = null;
					}
				}
			}
		}

		private void PositionItemInCell(GameObject item, int originX, int originY, Vector2Int size, int index) {
			item.transform.SetParent(transform);
			item.transform.localRotation = Quaternion.identity;
			var cellCenter = GetMultiCellLocalCenter(originX, originY, size);
			item.transform.localPosition = cellCenter + new Vector3(0f, index * _stackVerticalOffset, 0f);
		}
		private Vector3 GetMultiCellLocalCenter(int originX, int originY, Vector2Int size) {
			var minX = originX;
			var maxX = originX + size.x - 1;
			var minY = originY;
			var maxY = originY + size.y - 1;
			var centerX = (minX + maxX) / 2f;
			var centerY = (minY + maxY) / 2f;
			return GetCellLocalCenter(centerX, centerY);
		}
		private Vector3 GetCellLocalCenter(float x, float y) {
			var offsetX = (x - _gridSize.x / 2f + 0.5f) * _cellSize.x;
			var offsetZ = (y - _gridSize.y / 2f + 0.5f) * _cellSize.y;
			return new Vector3(offsetX, 0f, offsetZ);
		}

		public override bool CanTake() {
			for (var x = _gridSize.x - 1; x >= 0; x--) {
				for (var y = _gridSize.y - 1; y >= 0; y--) {
					var rack = _grid[x, y];
					if (rack != null && rack.CanTake()) {
						return true;
					}
				}
			}
			return false;
		}
		public override GameObject Take() {
			for (var x = _gridSize.x - 1; x >= 0; x--) {
				for (var y = _gridSize.y - 1; y >= 0; y--) {
					var rack = _grid[x, y];
					if (rack != null && rack.CanTake()) {
						var originX = -1;
						var originY = -1;
						for (var cx = 0; cx < _gridSize.x && originX < 0; cx++) {
							for (var cy = 0; cy < _gridSize.y && originY < 0; cy++) {
								if (_grid[cx, cy] == rack) {
									originX = cx;
									originY = cy;
								}
							}
						}

						var takenItem = rack.Take();
						if (takenItem == null) {
							continue;
						}

						if (!rack.CanTake()) {
							ClearRackCells(rack);
						}

						takenItem.transform.SetParent(null);
						return takenItem;
					}
				}
			}
			return null;
		}

		private void OnDrawGizmos() {
			var matrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			for (var x = 0; x < _gridSize.x; x++) {
				for (var y = 0; y < _gridSize.y; y++) {
					var cellCenter = GetCellLocalCenter(x, y);
					var rack = _grid?[x, y];
					var color = Color.white;

					if (rack != null) {
						if (rack.IsFull) {
							color = Color.red;
						} else {
							color = Color.green;
						}
					}
					color.a = 0.3f;

					Gizmos.color = color;
					Gizmos.DrawCube(cellCenter, new Vector3(_cellSize.x, 0.05f, _cellSize.y));

					Gizmos.color = Color.black;
					Gizmos.DrawWireCube(cellCenter, new Vector3(_cellSize.x, 0.05f, _cellSize.y));
				}
			}

			Gizmos.matrix = matrix;
		}
	}
}
