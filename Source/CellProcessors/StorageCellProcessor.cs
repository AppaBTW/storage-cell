using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    [LockRotation]
    public class StorageCellProcessor : CellProcessor
    {
        public override string Name => "Storage Cell";
        public override int CellType => 11;
        public override string CellSpriteIndex => "Storage";

        public StorageCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }
        public bool DoRegularMove(BasicCell cell, Direction direction, int force)
        {
            if (force <= 0)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);

            if (targetCell is null)
            {
                _cellGrid.MoveCell(cell, target);
                return true;
            }

            if (!_cellGrid.PushCell(targetCell.Value, direction, force))
                return false;

            _cellGrid.MoveCell(cell, target);
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (cell.Frozen)
                return DoRegularMove(cell, direction, force);
            if (force <= 0)
                return false;
            BasicCell useCell;
            useCell = cell;
            var pusherpos = cell.Transform.Position - direction.AsVector2Int;
            var pusher = _cellGrid.GetCell(pusherpos);
            var targetCellpos = cell.Transform.Position + direction.AsVector2Int;
            var targetCell = _cellGrid.GetCell(pos: targetCellpos);
            if (pusher.Value.Instance.Type == 11)
                return DoRegularMove(cell, direction, force);
            if (useCell.SpriteVariant != 0)
            {
                if (targetCell != null)
                    if (!_cellGrid.PushCell((BasicCell)targetCell, targetCell.Value.Transform.Direction, 1))
                return false;
                _cellGrid.AddCell(targetCellpos, cell.Transform.Direction, cell.SpriteVariant - 1, cell.Transform);
            }
            
            _cellGrid.RemoveCell(useCell);
            useCell.Transform = useCell.Transform.SetDirection(pusher.Value.Transform.Direction);
            useCell.PreviousTransform = useCell.Transform;
            useCell.SpriteVariant = (short)(pusher.Value.Instance.Type + 1);
            _cellGrid.RemoveCell((BasicCell)pusher);
            _cellGrid.AddCell(useCell);

            return false;
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
            //do nothing
        }

        public override void Clear()
        {
            //do nothing
        }
    }
}