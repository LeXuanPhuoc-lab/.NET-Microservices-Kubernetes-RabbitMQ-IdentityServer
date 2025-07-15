import { Button, ButtonGroup } from 'flowbite-react';
import React from 'react'
import { useParamsStore } from '../hooks/useParamsStore';
import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai';
import { BsFillStopCircleFill, BsStopwatchFill } from 'react-icons/bs';
import { GiFlame, GiFinishLine } from 'react-icons/gi';

const pageSizeButtons = [4, 8, 12];

const orderButtons = [
    {
        label: 'Alphabetical',
        icon: AiOutlineSortAscending,
        value: 'MAKE'
    },
    {
        label: 'End date',
        icon: AiOutlineClockCircle,
        value: 'ENDINGSOON'
    },
    {
        label: 'Recently added',
        icon: BsFillStopCircleFill,
        value: 'NEW'
    }
];

const filterButtons = [
    {
        label: 'Live Auctions',
        icon: GiFlame,
        value: 'LIVE'
    },
    {
        label: 'Ending < 6 hours',
        icon: GiFinishLine,
        value: 'ENDINGSOON'
    },
    {
        label: 'Completed',
        icon: BsStopwatchFill,
        value: 'FINISHED'
    }
];

export default function Filters() {
    const pageSize = useParamsStore(state => state.pageSize);
    const setParams = useParamsStore(state => state.setParams);
    const orderBy = useParamsStore(state => state.orderBy);
    const filterBy = useParamsStore(state => state.filterBy);

    return (
        <div className='flex justify-between mb-4 items-center'>
            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Filter by</span>
                <ButtonGroup>
                    {filterButtons.map(({ label, icon: Icon, value }) => (
                        <Button
                            key={value}
                            onClick={() => setParams({ filterBy: value })}
                            color={`${filterBy === value ? 'red' : 'gray'}`}
                        >
                            <Icon className='mr-3 h-4 w-4' />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>

            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Order by</span>
                <ButtonGroup>
                    {orderButtons.map(({ label, icon: Icon, value }) => (
                        <Button
                            key={value}
                            onClick={() => setParams({ orderBy: value })}
                            color={`${orderBy === value ? 'red' : 'gray'}`}
                        >
                            <Icon className='mr-3 h-4 w-4' />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
            <div>
                <span className='uppercase text-sm text-gray-500 mr-2'>Page size</span>
                <ButtonGroup>
                    {pageSizeButtons.map((value, index) => (
                        <Button
                            key={index}
                            onClick={() => setParams({ pageSize: value })}
                            color={`${pageSize === value ? 'red' : 'gray'}`}
                            className='focus:ring-0'
                        >
                            {value}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    )
}
