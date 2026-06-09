import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  
  return `${year}/${month}/${day} ${hours}:${minutes}`;
}

export function getBgColorFromId(id: number): string {
  const colors = [
    '#1f77b4',
    '#ff7f0e',
    '#2ca02c',
    '#d62728',
    '#9467bd',
    '#8c564b',
    '#e377c2',
    '#7f7f7f',
    '#bcbd22',
    '#17becf' 
  ]
  return colors[id % colors.length]
}

/*
const randomColors = [
  '#3d1f1e',
  '#564327',
  '#e5d695',
  '#998538',
  '#84376e',
  '#362d56',
  '#1c0e21',
  '#9ba58e',
  '#b9d1a5',
  '#bc9b8b',
  '#b962d1',
  '#191415',
  '#1d2323',
  '#0e681c',
  '#425124',
  '#b54d53',
  '#2b4c0d',
  '#1e1c18',
  '#af8483',
  '#499650',
  '#b1c4d3',
  '#f293d9',
  '#03000f',
  '#e81b52',
  '#636637',
  '#4b307a',
  '#916187',
  '#4f8c78',
  '#085131',
  '#010202',
  '#272819',
  '#040501',
  '#333107',
  '#111604',
  '#404c17',
  '#1e1e1e',
  '#16385b',
  '#efe939',
  '#7c7b7c',
  '#ade004',
  '#d8d16a',
  '#0c1e13',
  '#1b2326',
  '#591810',
  '#262319',
  '#706f6b',
  '#e782ed',
  '#934a05',
  '#225559',
  '#5f8e83'
]
*/