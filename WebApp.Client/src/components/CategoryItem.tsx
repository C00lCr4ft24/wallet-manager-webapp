import { Item, ItemMedia, ItemContent, ItemTitle } from "@/components/ui/item";
import { Avatar } from "primereact/avatar";


function CategoryItem({ name, icon, color }: { name: string; icon?: string; color?: string }) {
  return (
    <div style={{ backgroundColor: color ? "#00000000" : 'transparent' }} className="rounded-md">
    <Item variant="default" className="w-full" style={{ backgroundColor: color ? color + "90" : 'transparent' }}>
      <ItemMedia>
        <Avatar label={icon ? icon : name?.charAt(0).toUpperCase() || '?'} shape="circle" size="large" style={{ backgroundColor: color ?? 'transparent' }} />
      </ItemMedia>
      <ItemContent>
        <ItemTitle>{name}</ItemTitle>
      </ItemContent>
    </Item>
    </div>
  )
}

export default CategoryItem