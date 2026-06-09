import { ColorPicker, type ColorPickerHSBType, type ColorPickerRGBType } from 'primereact/colorpicker';
import categoryService from "@/services/categoryService";
import { toast } from "sonner";
import { useEffect, useState } from 'react';
import { CategoryHeaderDto, CreateCategoryDto, ProblemDetails } from '@/generated/dtos';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Button } from './ui/button';
import { Field, FieldGroup } from './ui/field';
import { Input } from './ui/input';
import { Label } from './ui/label';


function CategoryDialog({ children, onChanges, category }: { children: React.ReactNode; onChanges: () => void; category?: CategoryHeaderDto }) {
  const [open, setOpen] = useState(false)

  const [name, setName] = useState<string>('')
  const [description, setDescription] = useState<string | undefined>(undefined)
  const [icon, setIcon] = useState<string | undefined>(undefined)
  const [color, setColor] = useState<string | ColorPickerRGBType | ColorPickerHSBType | undefined>(undefined)

  const convertColorToHex = (color: string | ColorPickerRGBType | ColorPickerHSBType | undefined): string | undefined => {
    if (typeof color === 'string') {
      return "#" + color
    }
    return undefined
  }

  const isEditMode = !!category

  useEffect(() => {
    if (category) {
      setName(category.name!)
      setDescription(category.description)
      setIcon(category.icon)
      setColor(category.color)
    }
  }, [category])

  const handleDelete = async () => {
    if (!category) return
    try {
      await categoryService.DeleteCategory(category.id!)
      onChanges()
      toast.success("Category deleted successfully!", { position: 'bottom-center' })
      setOpen(false)
    }
    catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  const handleSubmit = async () => {
    if (!name) {
      toast.error("Name is required!", { position: 'bottom-center' })
      return
    }
    try {
      if (isEditMode && category) {
        await categoryService.UpdateCategory(
          category.id!,
          new CreateCategoryDto({
            name: name,
            description: description,
            icon: icon,
            color: convertColorToHex(color)
          })
        )
        toast.success("Category updated successfully!", { position: 'bottom-center' })
      }
      if (!isEditMode) {
        await categoryService.CreateCategory(
          new CreateCategoryDto({
            name: name,
            description: description,
            icon: icon,
            color: convertColorToHex(color)
          })
        )
        toast.success("Category created successfully!", { position: 'bottom-center' })
      }
      onChanges()
      setOpen(false)
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
    finally {
      setColor(undefined)
      setDescription(undefined)
      setIcon(undefined)
      setName('')
    }
  }

  return (
    <Dialog open={open} onOpenChange={(isOpen) => setOpen(isOpen)}>
      <form>
        <DialogTrigger asChild>
          {children}
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>{isEditMode ? "Edit Category" : "Add Category"}</DialogTitle>
          </DialogHeader>
          <FieldGroup>
            <Field>
              <Label htmlFor="name">Name*</Label>
              <Input id="name" name="name" type="text" value={name} onChange={(e) => setName(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
            <Field>
              <Label htmlFor="description">Description</Label>
              <Input id="description" name="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
            </Field>
            <Field>
              <Label htmlFor="icon">Icon</Label>
              <Input id="icon" name="icon" type="text" value={icon} onChange={(e) => setIcon(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
            <Field>
              <Label htmlFor="color">Color</Label>
              <ColorPicker id="color" name="color" format='hex' value={color} onChange={(e) => setColor(e.value === null ? undefined : e.value)} inline />
              <ColorPicker id="color" name="color" format='hex' value={color} onChange={(e) => setColor(e.value === null ? undefined : e.value)} disabled />
            </Field>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline">Cancel</Button>
            </DialogClose>
            {isEditMode ? (
              <>
                <Button variant="destructive" type="submit" onClick={() => { handleDelete() }}>
                  Delete
                </Button>
                <Button type="submit" onClick={() => { handleSubmit() }}>
                  Save
                </Button>
              </>
            ) : (
              <Button type="submit" onClick={() => { handleSubmit() }}>
                Create
              </Button>
            )}
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default CategoryDialog