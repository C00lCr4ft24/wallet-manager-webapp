
import { Avatar } from 'primereact/avatar';
import {
  Item,
  ItemContent,
  ItemMedia,
  ItemTitle,
} from "@/components/ui/item"
import { getBgColorFromId } from "@/lib/utils";

function UserAvatarItem({ userId, userName, size }: { userId: number; userName: string; size?: 'large' | 'normal' | 'xlarge' }) {
  return (
    <Item variant="default" className="w-full">
      <ItemMedia>
        <Avatar
          label={userName?.charAt(0).toUpperCase() || '?'}
          shape="circle"
          size={size === 'large' ? 'large' : size === 'normal' ? 'normal' : 'xlarge'}
          className=""
          style={{ backgroundColor: getBgColorFromId(userId), color: '#fff' }}
        />
      </ItemMedia>
      <ItemContent>
        <ItemTitle>{userName}</ItemTitle>
      </ItemContent>
    </Item>
  )
}

export default UserAvatarItem